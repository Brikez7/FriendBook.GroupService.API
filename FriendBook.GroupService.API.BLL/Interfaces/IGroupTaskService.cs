using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.Response;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IGroupTaskService
    {
        public Task<BaseResponse<ResponseGroupTaskView>> CreateGroupTask(RequestGroupTaskNew requestGroupTaskNew, Guid adminId,string adminLogin);
        public Task<BaseResponse<GroupTask>> UpdateGroupTask(RequestGroupTaskChanged requestGroupTaskChanged, Guid adminId);
        public Task<BaseResponse<bool>> DeleteGroupTask(RequestGroupTaskKey requestGroupTaskKey, Guid adminId);
        public BaseResponse<IQueryable<GroupTask>> GetGroupTaskOData();
        public Task<BaseResponse<GroupTask>> SubcsribeGroupTask(RequestGroupTaskKey groupTaskKeyDTO, Guid userId);
        public Task<BaseResponse<GroupTask>> UnsubcsribeGroupTask(RequestGroupTaskKey groupTaskKeyDTO, Guid userId);
        public Task<BaseResponse<int>> UpdateStatusInGroupTasks();
    }
}
