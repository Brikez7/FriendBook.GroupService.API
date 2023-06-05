using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IGroupTaskService
    {
        public Task<BaseResponse<GroupTask>> CreateGroupTask(GroupTask group);
        public Task<BaseResponse<GroupTask>> UpdateGroupTask(GroupTask group,string newName,Guid userId);
        public Task<BaseResponse<bool>> DeleteGroupTask(GroupTask deletedGroup, Guid userId);
        public BaseResponse<IQueryable<GroupTask>> GetGroupTaskOData();
        public Task<BaseResponse<GroupTask>> SubcsribeGroupTask(GroupTask groupTask, Guid userId);
        public Task<BaseResponse<GroupTask>> UnsubcsribeGroupTask(GroupTask groupTask, Guid userId);
    }
}
