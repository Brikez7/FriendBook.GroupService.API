using FriendBook.GroupService.API.Domain.DTO.GroupTasksDTO;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IGroupTaskService
    {
        public Task<BaseResponse<GroupTaskViewDTO>> CreateGroupTask(GroupTaskNewDTO group, Guid userId,string login);
        public Task<BaseResponse<GroupTask>> UpdateGroupTask(GroupTaskChangedDTO group, Guid userId);
        public Task<BaseResponse<bool>> DeleteGroupTask(GroupTaskKeyDTO deletedGroup, Guid userId);
        public BaseResponse<IQueryable<GroupTask>> GetGroupTaskOData();
        public Task<BaseResponse<GroupTask>> SubcsribeGroupTask(GroupTaskKeyDTO groupTaskKeyDTO, Guid userId);
        public Task<BaseResponse<GroupTask>> UnsubcsribeGroupTask(GroupTaskKeyDTO groupTaskKeyDTO, Guid userId);
    }
}
