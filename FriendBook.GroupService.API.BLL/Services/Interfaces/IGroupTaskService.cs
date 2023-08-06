using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.Response;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IGroupTaskService
    {
        public Task<BaseResponse<ResponseGroupTaskView>> CreateGroupTask(RequestNewGroupTask requestGroupTaskNew, Guid adminId,string adminLogin);
        public Task<BaseResponse<RequestGroupTaskChanged>> UpdateGroupTask(RequestGroupTaskChanged requestGroupTaskChanged, Guid adminId);
        public Task<BaseResponse<bool>> DeleteGroupTask(RequestGroupTaskKey requestGroupTaskKey, Guid adminId);
        public BaseResponse<IQueryable<GroupTask>> GetGroupTaskOData();
        public Task<BaseResponse<bool>> SubscribeGroupTask(RequestGroupTaskKey groupTaskKeyDTO, Guid userId);
        public Task<BaseResponse<bool>> UnsubscribeGroupTask(RequestGroupTaskKey groupTaskKeyDTO, Guid userId);
        public Task<BaseResponse<int>> UpdateStatusInGroupTasks();
    }
}
