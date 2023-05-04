using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.InnerResponse;
using FriendBook.GroupService.API.Domain;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.BLL.Interfaces;

namespace FriendBook.GroupService.API.BLL.Services
{
    public class GroupTaskService : IGroupTaskService
    {
        private readonly IGroupTaskRepository _groupTaskRepository;

        public GroupTaskService(IGroupTaskRepository groupTaskRepository)
        {
            _groupTaskRepository = groupTaskRepository;
        }

        public async Task<BaseResponse<GroupTask>> CreateGroupTask(GroupTask groupTask)
        {
            var createdGroup = await _groupTaskRepository.AddAsync(groupTask);
            await _groupTaskRepository.SaveAsync();

            return new StandartResponse<GroupTask>()
            {
                Data = createdGroup,
                StatusCode = StatusCode.GroupCreate
            };
        }

        public async Task<BaseResponse<bool>> DeleteGroupTask(Guid id)
        {
            var Result = _groupTaskRepository.Delete(new GroupTask(id));
            await _groupTaskRepository.SaveAsync();

            return new StandartResponse<bool>()
            {
                Data = Result,
                StatusCode = StatusCode.GroupDelete
            };
        }

        public BaseResponse<IQueryable<GroupTask>> GetGroupTaskOData()
        {
            var groupTasks = _groupTaskRepository.Get();
            if (groupTasks.Count() == 0)
            {
                return new StandartResponse<IQueryable<GroupTask>>()
                {
                    Message = "entity not found"
                };
            }

            return new StandartResponse<IQueryable<GroupTask>>()
            {
                Data = groupTasks,
                StatusCode = StatusCode.GroupRead
            };
        }

        public async Task<BaseResponse<GroupTask>> UpdateGroupTask(GroupTask groupTask)
        {
            var updatedGroup = _groupTaskRepository.Update(groupTask);
            await _groupTaskRepository.SaveAsync();

            return new StandartResponse<GroupTask>()
            {
                Data = updatedGroup,
                StatusCode = StatusCode.GroupUpdate
            };
        }
    }
}
