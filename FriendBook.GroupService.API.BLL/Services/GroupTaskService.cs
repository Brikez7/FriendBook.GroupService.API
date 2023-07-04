using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.API.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using MongoDB.Driver;
using FriendBook.GroupService.API.Domain.Entities.MongoDB;

namespace FriendBook.GroupService.API.BLL.Services
{
    public class GroupTaskService : IGroupTaskService
    {
        private readonly IGroupTaskRepository _groupTaskRepository;
        private readonly IAccountStatusGroupRepository _accountStatusGroupRepository;
        private readonly IStageGroupTaskRepository _repositoryStageTask;
        public GroupTaskService(IGroupTaskRepository groupTaskRepository, IAccountStatusGroupRepository accountStatusGroupRepository, IStageGroupTaskRepository stageGroupTask)
        {
            _groupTaskRepository = groupTaskRepository;
            _accountStatusGroupRepository = accountStatusGroupRepository;
            _repositoryStageTask = stageGroupTask;
        }

        public async Task<BaseResponse<ResponseGroupTaskView>> CreateGroupTask(RequestGroupTaskNew groupTask,Guid userId,string login)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == userId && x.IdGroup == groupTask.GroupId && x.RoleAccount > RoleAccount.Default))
            {
                return new StandartResponse<ResponseGroupTaskView> 
                {
                    Message = "Account in group not found or you not access in group",
                    StatusCode = StatusCode.UserNotAccess
                };
            }

            if (await _groupTaskRepository.GetAll().AnyAsync(x => x.Name == groupTask.Name && x.GroupId == groupTask.GroupId))
            {
                return new StandartResponse<ResponseGroupTaskView>
                {
                    Message = "Task in with name exists",
                    StatusCode = StatusCode.GroupTaskExists
                };
            }

            var newGroupTask = new GroupTask(groupTask, userId);
            var createdGroup = await _groupTaskRepository.AddAsync(newGroupTask);
            await _groupTaskRepository.SaveAsync();

            var stageGroupTask = new StageGroupTask(MongoDB.Bson.ObjectId.GenerateNewId(), (Guid)createdGroup.Id!, $"Start task: {createdGroup.Name}", "", DateTime.UtcNow);
            var result = await _repositoryStageTask.AddAsync(stageGroupTask);

            var viewDTO = new ResponseGroupTaskView(createdGroup)
            {
                Users = new string[] { login }
            };

            return new StandartResponse<ResponseGroupTaskView>()
            {
                Data = viewDTO,
                StatusCode = StatusCode.GroupCreate
            };
        }

        public async Task<BaseResponse<GroupTask>> SubcsribeGroupTask(RequestGroupTaskKey groupTaskKeyDTO, Guid userId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == groupTaskKeyDTO.GroupId && userId == x.AccountId)) 
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "Group not exists or you not exists in group",
                    StatusCode = StatusCode.UserNotExists
                };
            }

            var task = await _groupTaskRepository.GetAll()
                                                 .Where(x => x.GroupId == groupTaskKeyDTO.GroupId && x.Name == groupTaskKeyDTO.Name)
                                                 .FirstOrDefaultAsync();

            if (task is null)
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "task not exists",
                    StatusCode = StatusCode.InternalServerError
                };
            }
            if (task.Team.Any(t => t == userId)) 
            {
                return new StandartResponse<GroupTask> { Message = "You already subscribe in group", StatusCode =  StatusCode.SubscribeErrror};
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
        public async Task<BaseResponse<GroupTask>> UnsubcsribeGroupTask(RequestGroupTaskKey groupTaskKeyDTO, Guid userId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == groupTaskKeyDTO.GroupId && userId == x.AccountId))
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "Group not exists or you not been in group",
                    StatusCode = StatusCode.UserNotExists
                };
            }

            var task = await _groupTaskRepository.GetAll()
                                                 .Where(x => x.GroupId == groupTaskKeyDTO.GroupId && x.Name == groupTaskKeyDTO.Name)
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
            if (groupTasks.Any())
            {
                return new StandartResponse<IQueryable<GroupTask>>()
                {
                    Message = "entity not found",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            return new StandartResponse<IQueryable<GroupTask>>()
            {
                Data = groupTasks,
                StatusCode = StatusCode.GroupRead
            };
        }

        public async Task<BaseResponse<GroupTask>> UpdateGroupTask(RequestGroupTaskChanged groupTask,Guid userId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == groupTask.GroupId && x.AccountId == userId && x.RoleAccount > RoleAccount.Default)) 
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "You are not exists in this group or you do not have access",
                    StatusCode = StatusCode.UserNotAccess
                };
            }

            var tasks = _groupTaskRepository.GetAll().Where(x => x.GroupId == groupTask.GroupId).AsQueryable();
            var task = await tasks.FirstOrDefaultAsync(x => groupTask.OldName == x.Name);

            if (task is null)
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "Task not found",
                    StatusCode = StatusCode.EntityNotFound
                };
            }
            else if (await tasks.AnyAsync(x => x.Name == groupTask.NewName) && groupTask.NewName != groupTask.OldName)
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "The task with name already exists",
                    StatusCode = StatusCode.GroupTaskExists
                };
            }

            task.Status = groupTask.Status;
            task.DateEndWork = groupTask.DateEndWork;
            task.Description = groupTask.Description;
            task.Name = groupTask.NewName;

            var updatedGroupTask = _groupTaskRepository.Update(task);
            await _groupTaskRepository.SaveAsync();

            return new StandartResponse<GroupTask>()
            {
                Data = updatedGroupTask,
                StatusCode = StatusCode.GroupTaskUpdate
            };
        }

        public async Task<BaseResponse<bool>> DeleteGroupTask(RequestGroupTaskKey deletedGroupTask, Guid userId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == deletedGroupTask.GroupId && x.AccountId == userId && x.RoleAccount > RoleAccount.Default))
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
                    Message = "This task not exists",
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
