using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.InnerResponse;
using FriendBook.GroupService.API.Domain;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.GroupService.API.BLL.Services
{
    public class GroupTaskService : IGroupTaskService
    {
        private readonly IGroupTaskRepository _groupTaskRepository;
        private readonly IAccountStatusGroupRepository _accountStatusGroupRepository;
        public GroupTaskService(IGroupTaskRepository groupTaskRepository, IAccountStatusGroupRepository accountStatusGroupRepository)
        {
            _groupTaskRepository = groupTaskRepository;
            _accountStatusGroupRepository = accountStatusGroupRepository;
        }

        public async Task<BaseResponse<GroupTask>> CreateGroupTask(GroupTask groupTask)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == groupTask.CreaterId && x.IdGroup == groupTask.GroupId && x.RoleAccount > RoleAccount.Default))
            {
                return new StandartResponse<GroupTask> 
                {
                    Message = "Account in group not found",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            if (await _groupTaskRepository.Get().AnyAsync(x => x.Name == groupTask.Name && x.GroupId == groupTask.GroupId))
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "Task in with name exists",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            var createdGroup = await _groupTaskRepository.AddAsync(groupTask);
            await _groupTaskRepository.SaveAsync();

            return new StandartResponse<GroupTask>()
            {
                Data = createdGroup,
                StatusCode = StatusCode.GroupCreate
            };
        }

        public async Task<BaseResponse<GroupTask>> SubcsribeGroupTask(GroupTask groupTask, Guid userId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == groupTask.GroupId && userId == x.AccountId)) 
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "Group not exists or you not been in group",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            var task = await _groupTaskRepository.Get()
                                                  .Where(x => x.GroupId == groupTask.GroupId && x.Name == groupTask.Name)
                                                  .FirstOrDefaultAsync();

            if (task is null || task.Team.Any(t => t == userId))
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "You been in group or task not exists",
                    StatusCode = StatusCode.InternalServerError
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
        public async Task<BaseResponse<GroupTask>> UnsubcsribeGroupTask(GroupTask groupTask, Guid userId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == groupTask.GroupId && userId == x.AccountId))
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "Group not exists or you not been in group",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            var task = await _groupTaskRepository.Get()
                                                  .Where(x => x.GroupId == groupTask.GroupId && x.Name == groupTask.Name)
                                                  .FirstOrDefaultAsync();

            if (task is null || !task.Team.Any(t => t == userId))
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "You been in group or task not exists",
                    StatusCode = StatusCode.InternalServerError
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
            var groupTasks = _groupTaskRepository.Get();
            if (groupTasks.Count() == 0)
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

        public async Task<BaseResponse<GroupTask>> UpdateGroupTask(GroupTask groupTask,string newNameTask,Guid userId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == groupTask.GroupId && x.AccountId == userId && x.RoleAccount > RoleAccount.Default)) 
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "You are not in this group or you do not have access",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            var tasks = _groupTaskRepository.Get().Where(x => x.GroupId == groupTask.GroupId).AsQueryable();
            var task = await tasks.FirstOrDefaultAsync(x => groupTask.Name == x.Name);

            if (task is null)
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "Task not found",
                    StatusCode = StatusCode.InternalServerError
                };
            }
            else if (await tasks.AnyAsync(x => x.Name == newNameTask) && newNameTask != groupTask.Name)
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "The task with name already exists",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            task.Status = groupTask.Status;
            task.DateStartWork = groupTask.DateEndWork;
            task.Description = groupTask.Description;
            task.Name = newNameTask;

            var updatedGroupTask = _groupTaskRepository.Update(task);
            await _groupTaskRepository.SaveAsync();

            return new StandartResponse<GroupTask>()
            {
                Data = updatedGroupTask,
                StatusCode = StatusCode.GroupUpdate
            };
        }

        public async Task<BaseResponse<bool>> DeleteGroupTask(GroupTask deletedGroupTask, Guid userId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == deletedGroupTask.GroupId && x.AccountId == userId && x.RoleAccount > RoleAccount.Default))
            {
                return new StandartResponse<bool>
                {
                    Message = "You are not in this group or you do not have access that delete task",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            var deletedTask = await _groupTaskRepository.Get().FirstOrDefaultAsync(x => x.GroupId == deletedGroupTask.GroupId && x.Name == deletedGroupTask.Name && x.Status > StatusTask.Process);
            if(deletedTask is null)
            {
                return new StandartResponse<bool>
                {
                    Message = "This task not exit",
                    StatusCode = StatusCode.InternalServerError
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
    }
}
