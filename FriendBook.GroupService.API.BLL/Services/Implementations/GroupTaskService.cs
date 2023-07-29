using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.API.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using MongoDB.Driver;
using FriendBook.GroupService.API.Domain.Entities.MongoDB;
using FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs;

namespace FriendBook.GroupService.API.BLL.Services
{
    public class GroupTaskService : IGroupTaskService
    {
        private readonly IGroupTaskRepository _groupTaskRepository;
        private readonly IAccountStatusGroupRepository _accountStatusGroupRepository;
        private readonly IStageGroupTaskRepository _stageGroupTaskRepository;
        public GroupTaskService(IGroupTaskRepository groupTaskRepository, IAccountStatusGroupRepository accountStatusGroupRepository, IStageGroupTaskRepository stageGroupTask)
        {
            _groupTaskRepository = groupTaskRepository;
            _accountStatusGroupRepository = accountStatusGroupRepository;
            _stageGroupTaskRepository = stageGroupTask;
        }

        public async Task<BaseResponse<ResponseGroupTaskView>> CreateGroupTask(RequestGroupTaskNew requestGroupTaskNew, Guid adminId, string loginAdmin)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == adminId && x.IdGroup == requestGroupTaskNew.GroupId && x.RoleAccount > RoleAccount.Default))
            {
                return new StandardResponse<ResponseGroupTaskView> 
                {
                    Message = "Account not found or you not access create new group task",
                    ServiceCode = ServiceCode.UserNotAccess
                };
            }

            if (await _groupTaskRepository.GetAll().AnyAsync(x => x.Name == requestGroupTaskNew.Name && x.GroupId == requestGroupTaskNew.GroupId))
            {
                return new StandardResponse<ResponseGroupTaskView>
                {
                    Message = "Task with name already exists",
                    ServiceCode = ServiceCode.GroupTaskAlreadyExists
                };
            }

            var newGroupTask = new GroupTask(requestGroupTaskNew, adminId);
            var createdGroup = await _groupTaskRepository.AddAsync(newGroupTask);
            await _groupTaskRepository.SaveAsync();

            var stageGroupTask = new StageGroupTask(MongoDB.Bson.ObjectId.GenerateNewId(), (Guid)createdGroup.Id!, $"Start task: {createdGroup.Name}", "", DateTime.UtcNow);
            var result = await _stageGroupTaskRepository.AddAsync(stageGroupTask);

            var listStage = new List<ResponseStageGroupTaskIcon>() { new ResponseStageGroupTaskIcon(result.Id, result.Name, result.IdGroupTask) };
            var viewDTO = new ResponseGroupTaskView(createdGroup, listStage)
            {
                Users = new string[] { loginAdmin }
            };

            return new StandardResponse<ResponseGroupTaskView>()
            {
                Data = viewDTO,
                ServiceCode = ServiceCode.GroupCreated
            };
        }

        public async Task<BaseResponse<bool>> SubcsribeGroupTask(RequestGroupTaskKey requestGroupTaskKey, Guid userId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == requestGroupTaskKey.GroupId && userId == x.AccountId)) 
            {
                return new StandardResponse<bool>
                {
                    Message = "Group not exists or you not exists in group",
                    ServiceCode = ServiceCode.UserNotExists
                };
            }

            var task = await _groupTaskRepository.GetAll()
                                                 .Where(x => x.Id == requestGroupTaskKey.GroupTaskId)
                                                 .FirstOrDefaultAsync();

            if (task is null)
            {
                return new StandardResponse<bool>
                {
                    Message = "task not exists",
                    ServiceCode = ServiceCode.EntityNotFound
                };
            }
            if (task.Team.Any(t => t == userId))
            {
                return new StandardResponse<bool>
                {
                    Message = "You already subscribe in group", 
                    ServiceCode = ServiceCode.SubscribeTaskError
                };
            }

            task.Team = task.Team.Append(userId).ToArray();

            var updatedTask = _groupTaskRepository.Update(task);
            await _groupTaskRepository.SaveAsync();

            return new StandardResponse<bool>()
            {
                Data = updatedTask != null,
                ServiceCode = ServiceCode.GroupUpdated
            };
        }
        public async Task<BaseResponse<bool>> UnsubcsribeGroupTask(RequestGroupTaskKey requestGroupTaskKey, Guid userId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == requestGroupTaskKey.GroupId && userId == x.AccountId))
            {
                return new StandardResponse<bool>
                {
                    Message = "Group not exists or you not been in group",
                    ServiceCode = ServiceCode.EntityNotFound
                };
            }

            var task = await _groupTaskRepository.GetAll()
                                                 .Where(x => x.Id == requestGroupTaskKey.GroupTaskId)
                                                 .FirstOrDefaultAsync();

            if (task is null)
            {
                return new StandardResponse<bool>
                {
                    Message = "Task not exists",
                    ServiceCode = ServiceCode.EntityNotFound
                };
            }
            if (!task.Team.Any(t => t == userId)) 
            {
                return new StandardResponse<bool>
                {
                    Message = "You already unsubscribe in group",
                    ServiceCode = ServiceCode.UnsubscribeTaskError
                };
            }

            task.Team = task.Team.Where(x => x != userId).ToArray();

            var updatedGroup = _groupTaskRepository.Update(task);
            await _groupTaskRepository.SaveAsync();

            return new StandardResponse<bool>()
            {
                Data = updatedGroup != null,
                ServiceCode = ServiceCode.GroupUpdated
            };
        }

        public BaseResponse<IQueryable<GroupTask>> GetGroupTaskOData()
        {
            var groupTasks = _groupTaskRepository.GetAll();

            return new StandardResponse<IQueryable<GroupTask>>()
            {
                Data = groupTasks,
                ServiceCode = ServiceCode.GroupReadied
            };
        }

        public async Task<BaseResponse<RequestGroupTaskChanged>> UpdateGroupTask(RequestGroupTaskChanged requestGroupTaskChanged,Guid adminId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == requestGroupTaskChanged.GroupId && x.AccountId == adminId && x.RoleAccount > RoleAccount.Default)) 
                return new StandardResponse<RequestGroupTaskChanged>
                {
                    Message = "You not exists in this group or you not have access update group task",
                    ServiceCode = ServiceCode.UserNotAccess
                };

            var tasks = _groupTaskRepository.GetAll().Where(x => x.GroupId == requestGroupTaskChanged.GroupId).AsQueryable();
            var task = await tasks.FirstOrDefaultAsync(x => requestGroupTaskChanged.OldName == x.Name);

            if (task is null)
            {
                return new StandardResponse<RequestGroupTaskChanged>
                {
                    Message = "Task not found",
                    ServiceCode = ServiceCode.EntityNotFound
                };
            }
            else if (await tasks.AnyAsync(x => x.Name == requestGroupTaskChanged.NewName) && requestGroupTaskChanged.NewName != requestGroupTaskChanged.OldName)
            {
                return new StandardResponse<RequestGroupTaskChanged>
                {
                    Message = "The task with name already exists",
                    ServiceCode = ServiceCode.GroupTaskAlreadyExists
                };
            }

            task.Status = requestGroupTaskChanged.Status;
            task.DateEndWork = requestGroupTaskChanged.DateEndWork;
            task.Description = requestGroupTaskChanged.Description;
            task.Name = requestGroupTaskChanged.NewName;

            var updatedGroupTask = _groupTaskRepository.Update(task);
            await _groupTaskRepository.SaveAsync();

            return new StandardResponse<RequestGroupTaskChanged>()
            {
                Data = requestGroupTaskChanged,
                ServiceCode = ServiceCode.GroupTaskUpdated
            };
        }

        public async Task<BaseResponse<bool>> DeleteGroupTask(RequestGroupTaskKey deletedGroupTask, Guid adminId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == deletedGroupTask.GroupId && x.AccountId == adminId && x.RoleAccount > RoleAccount.Default))
            {
                return new StandardResponse<bool>
                {
                    Message = "You are not in this group or you do not have access",
                    ServiceCode = ServiceCode.UserNotAccess
                };
            }

            var deletedTask = await _groupTaskRepository.GetAll().FirstOrDefaultAsync(x => x.Id == deletedGroupTask.GroupTaskId && x.Status > StatusTask.Process);
            if(deletedTask is null)
            {
                return new StandardResponse<bool>
                {
                    Message = "Task not exists",
                    ServiceCode = ServiceCode.EntityNotFound
                };
            }

            var Result = _groupTaskRepository.Delete(deletedTask);
            await _groupTaskRepository.SaveAsync();

            return new StandardResponse<bool>()
            {
                Data = Result,
                ServiceCode = ServiceCode.GroupDeleted
            };
        }

        public async Task<BaseResponse<int>> UpdateStatusInGroupTasks()
        {
            DateTime nowDate = DateTime.UtcNow.Date;
            int countUpdatedTask = await _groupTaskRepository.GetAll().Where(x => x.DateEndWork < nowDate && x.Status == StatusTask.Process).ExecuteUpdateAsync(x => x.SetProperty(prop => prop.Status, StatusTask.MissedDate));

            return new StandardResponse<int> { Data = countUpdatedTask, ServiceCode = ServiceCode.GroupTaskUpdated };
        }
    }
}
