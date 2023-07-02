using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.Response;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IGroupTaskService
    {
        public Task<BaseResponse<ResponseGroupTaskView>> CreateGroupTask(RequestGroupTaskNew group, Guid userId,string login);
        public Task<BaseResponse<GroupTask>> UpdateGroupTask(RequestGroupTaskChanged group, Guid userId);
        public Task<BaseResponse<bool>> DeleteGroupTask(RequestGroupTaskKey deletedGroup, Guid userId);
        public BaseResponse<IQueryable<GroupTask>> GetGroupTaskOData();
        public Task<BaseResponse<GroupTask>> SubcsribeGroupTask(RequestGroupTaskKey groupTaskKeyDTO, Guid userId);
        public Task<BaseResponse<GroupTask>> UnsubcsribeGroupTask(RequestGroupTaskKey groupTaskKeyDTO, Guid userId);
        public Task<BaseResponse<int>> UpdateStatusInGroupTasks();
    }
}
