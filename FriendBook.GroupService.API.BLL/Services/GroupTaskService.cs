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
                return new StandartResponse<ResponseGroupTaskView> 
                {
                    Message = "Account not found or you not access create new group task",
                    StatusCode = StatusCode.UserNotAccess
                };
            }

            if (await _groupTaskRepository.GetAll().AnyAsync(x => x.Name == requestGroupTaskNew.Name && x.GroupId == requestGroupTaskNew.GroupId))
            {
                return new StandartResponse<ResponseGroupTaskView>
                {
                    Message = "Task with name already exists",
                    StatusCode = StatusCode.GroupTaskAlreadyExists
                };
            }

            var newGroupTask = new GroupTask(requestGroupTaskNew, adminId);
            var createdGroup = await _groupTaskRepository.AddAsync(newGroupTask);
            await _groupTaskRepository.SaveAsync();

            var stageGroupTask = new StageGroupTask(MongoDB.Bson.ObjectId.GenerateNewId(), (Guid)createdGroup.Id!, $"Start task: {createdGroup.Name}", "", DateTime.UtcNow);
            var result = await _stageGroupTaskRepository.AddAsync(stageGroupTask);

            var listStage = new List<StageGroupTaskIconDTO>() { new StageGroupTaskIconDTO(result.Id, result.Name, result.IdGroupTask) };
            var viewDTO = new ResponseGroupTaskView(createdGroup, listStage)
            {
                Users = new string[] { loginAdmin }
            };

            return new StandartResponse<ResponseGroupTaskView>()
            {
                Data = viewDTO,
                StatusCode = StatusCode.GroupCreate
            };
        }

        public async Task<BaseResponse<GroupTask>> SubcsribeGroupTask(RequestGroupTaskKey requestGroupTaskKey, Guid userId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == requestGroupTaskKey.GroupId && userId == x.AccountId)) 
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "Group not exists or you not exists in group",
                    StatusCode = StatusCode.UserNotExists
                };
            }

            var task = await _groupTaskRepository.GetAll()
                                                 .Where(x => x.GroupId == requestGroupTaskKey.GroupId && x.Name == requestGroupTaskKey.Name)
                                                 .FirstOrDefaultAsync();

            if (task is null)
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "task not exists",
                    StatusCode = StatusCode.EntityNotFound
                };
            }
            if (task.Team.Any(t => t == userId))
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "You already subscribe in group", 
                    StatusCode = StatusCode.SubscribeErrror
                };
            }

            task.Team = task.Team.Append(userId).ToArray();

            var updatedTask = _groupTaskRepository.Update(task);
            await _groupTaskRepository.SaveAsync();

            return new StandartResponse<GroupTask>()
            {
                Data = updatedTask,
                StatusCode = StatusCode.GroupUpdate
            };
        }
        public async Task<BaseResponse<GroupTask>> UnsubcsribeGroupTask(RequestGroupTaskKey requestGroupTaskKey, Guid userId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == requestGroupTaskKey.GroupId && userId == x.AccountId))
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "Group not exists or you not been in group",
                    StatusCode = StatusCode.EntityNotFound
                };
            }

            var task = await _groupTaskRepository.GetAll()
                                                 .Where(x => x.GroupId == requestGroupTaskKey.GroupId && x.Name == requestGroupTaskKey.Name)
                                                 .FirstOrDefaultAsync();

            if (task is null)
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "Task not exists",
                    StatusCode = StatusCode.EntityNotFound
                };
            }
            if (!task.Team.Any(t => t == userId)) 
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "You already unsubscribe in group",
                    StatusCode = StatusCode.UnsubscribeError
                };
            }

            task.Team = task.Team.Where(x => x != userId).ToArray();

            var updatedGroup = _groupTaskRepository.Update(task);
            await _groupTaskRepository.SaveAsync();

            return new StandartResponse<GroupTask>()
            {
                Data = updatedGroup,
                StatusCode = StatusCode.GroupUpdate
            };
        }

        public BaseResponse<IQueryable<GroupTask>> GetGroupTaskOData()
        {
            var groupTasks = _groupTaskRepository.GetAll();

            return new StandartResponse<IQueryable<GroupTask>>()
            {
                Data = groupTasks,
                StatusCode = StatusCode.GroupRead
            };
        }

        public async Task<BaseResponse<GroupTask>> UpdateGroupTask(RequestGroupTaskChanged requestGroupTaskChanged,Guid adminId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == requestGroupTaskChanged.GroupId && x.AccountId == adminId && x.RoleAccount > RoleAccount.Default)) 
                return new StandartResponse<GroupTask>
                {
                    Message = "You not exists in this group or you not have access update group task",
                    StatusCode = StatusCode.UserNotAccess
                };

            var tasks = _groupTaskRepository.GetAll().Where(x => x.GroupId == requestGroupTaskChanged.GroupId).AsQueryable();
            var task = await tasks.FirstOrDefaultAsync(x => requestGroupTaskChanged.OldName == x.Name);

            if (task is null)
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "Task not found",
                    StatusCode = StatusCode.EntityNotFound
                };
            }
            else if (await tasks.AnyAsync(x => x.Name == requestGroupTaskChanged.NewName) && requestGroupTaskChanged.NewName != requestGroupTaskChanged.OldName)
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "The task with name already exists",
                    StatusCode = StatusCode.GroupTaskAlreadyExists
                };
            }

            task.Status = requestGroupTaskChanged.Status;
            task.DateEndWork = requestGroupTaskChanged.DateEndWork;
            task.Description = requestGroupTaskChanged.Description;
            task.Name = requestGroupTaskChanged.NewName;

            var updatedGroupTask = _groupTaskRepository.Update(task);
            await _groupTaskRepository.SaveAsync();

            return new StandartResponse<GroupTask>()
            {
                Data = updatedGroupTask,
                StatusCode = StatusCode.GroupTaskUpdate
            };
        }

        public async Task<BaseResponse<bool>> DeleteGroupTask(RequestGroupTaskKey deletedGroupTask, Guid adminId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == deletedGroupTask.GroupId && x.AccountId == adminId && x.RoleAccount > RoleAccount.Default))
            {
                return new StandartResponse<bool>
                {
                    Message = "You are not in this group or you do not have access",
                    StatusCode = StatusCode.UserNotAccess
                };
            }

            var deletedTask = await _groupTaskRepository.GetAll().FirstOrDefaultAsync(x => x.GroupId == deletedGroupTask.GroupId && x.Name == deletedGroupTask.Name && x.Status > StatusTask.Process);
            if(deletedTask is null)
            {
                return new StandartResponse<bool>
                {
                    Message = "Task not exists",
                    StatusCode = StatusCode.EntityNotFound
                };
            }

            var Result = _groupTaskRepository.Delete(deletedTask);
            await _groupTaskRepository.SaveAsync();

            return new StandartResponse<bool>()
            {
                Data = Result,
                StatusCode = StatusCode.GroupDelete
            };
        }

        public async Task<BaseResponse<int>> UpdateStatusInGroupTasks()
        {
            DateTime nowDate = DateTime.Now.Date;
            int countUpdatedTask = await _groupTaskRepository.GetAll().Where(x => x.DateEndWork < nowDate && x.Status == StatusTask.Process).ExecuteUpdateAsync(x => x.SetProperty(prop => prop.Status, StatusTask.MissedDate));

            if (countUpdatedTask == 0)
                return new StandartResponse<int> { Data = countUpdatedTask, StatusCode = StatusCode.GroupTaskNotUpdated, Message = "Count group task updated was equal to zero" };

            return new StandartResponse<int> { Data = countUpdatedTask, StatusCode = StatusCode.GroupTaskUpdate };
        }
    }
}
