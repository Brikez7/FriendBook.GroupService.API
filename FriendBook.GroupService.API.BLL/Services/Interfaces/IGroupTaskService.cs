using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Response;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IGroupTaskService
    {
        public Task<BaseResponse<ResponseGroupTaskView>> CreateGroupTask(RequestNewGroupTask requestGroupTaskNew, Guid adminId, string adminLogin);
        public Task<BaseResponse<UpdateGroupTaskDTO>> UpdateGroupTask(UpdateGroupTaskDTO requestGroupTaskChanged, Guid adminId);
        public Task<BaseResponse<bool>> DeleteGroupTask(Guid groupTaskId, Guid adminId);
        public Task<BaseResponse<bool>> SubscribeGroupTask(Guid groupTaskId, Guid userId);
        public Task<BaseResponse<bool>> UnsubscribeGroupTask(Guid groupTaskId, Guid userId);
        public Task<BaseResponse<int>> UpdateStatusInGroupTasks();
        public Task<BaseResponse<ResponseTasksPage>> GetTasksPage(string nameTask, Guid userId, Guid groupId);
    }
}
