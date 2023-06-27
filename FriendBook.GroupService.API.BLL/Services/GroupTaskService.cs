using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.InnerResponse;
using FriendBook.GroupService.API.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities;

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

        public async Task<BaseResponse<ResponseGroupTaskView>> CreateGroupTask(RequestGroupTaskNew groupTask,Guid userId,string login)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == userId && x.IdGroup == groupTask.GroupId && x.RoleAccount > RoleAccount.Default))
            {
                return new StandartResponse<ResponseGroupTaskView> 
                {
                    Message = "Account in group not found",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            if (await _groupTaskRepository.GetAll().AnyAsync(x => x.Name == groupTask.Name && x.GroupId == groupTask.GroupId))
            {
                return new StandartResponse<ResponseGroupTaskView>
                {
                    Message = "Task in with name exists",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            var newGroupTask = new GroupTask(groupTask, userId);
            var createdGroup = await _groupTaskRepository.AddAsync(newGroupTask);
            await _groupTaskRepository.SaveAsync();

            var viewDTO = new ResponseGroupTaskView(createdGroup);
            viewDTO.Users = new string[] { login };

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
                    Message = "Group not exists or you not been in group",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            var task = await _groupTaskRepository.GetAll()
                                                 .Where(x => x.GroupId == groupTaskKeyDTO.GroupId && x.Name == groupTaskKeyDTO.Name)
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
        public async Task<BaseResponse<GroupTask>> UnsubcsribeGroupTask(RequestGroupTaskKey groupTaskKeyDTO, Guid userId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == groupTaskKeyDTO.GroupId && userId == x.AccountId))
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "Group not exists or you not been in group",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            var task = await _groupTaskRepository.GetAll()
                                                 .Where(x => x.GroupId == groupTaskKeyDTO.GroupId && x.Name == groupTaskKeyDTO.Name)
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
            var groupTasks = _groupTaskRepository.GetAll();
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

        public async Task<BaseResponse<GroupTask>> UpdateGroupTask(RequestGroupTaskChanged groupTask,Guid userId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == groupTask.GroupId && x.AccountId == userId && x.RoleAccount > RoleAccount.Default)) 
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "You are not in this group or you do not have access",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            var tasks = _groupTaskRepository.GetAll().Where(x => x.GroupId == groupTask.GroupId).AsQueryable();
            var task = await tasks.FirstOrDefaultAsync(x => groupTask.OldName == x.Name);

            if (task is null)
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "Task not found",
                    StatusCode = StatusCode.InternalServerError
                };
            }
            else if (await tasks.AnyAsync(x => x.Name == groupTask.NewName) && groupTask.NewName != groupTask.OldName)
            {
                return new StandartResponse<GroupTask>
                {
                    Message = "The task with name already exists",
                    StatusCode = StatusCode.InternalServerError
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
                StatusCode = StatusCode.GroupUpdate
            };
        }

        public async Task<BaseResponse<bool>> DeleteGroupTask(RequestGroupTaskKey deletedGroupTask, Guid userId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == deletedGroupTask.GroupId && x.AccountId == userId && x.RoleAccount > RoleAccount.Default))
            {
                return new StandartResponse<bool>
                {
                    Message = "You are not in this group or you do not have access that delete task",
                    StatusCode = StatusCode.InternalServerError
                };
            }

            var deletedTask = await _groupTaskRepository.GetAll().FirstOrDefaultAsync(x => x.GroupId == deletedGroupTask.GroupId && x.Name == deletedGroupTask.Name && x.Status > StatusTask.Process);
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
