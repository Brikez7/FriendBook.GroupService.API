using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IGroupTaskService
    {
        public Task<BaseResponse<GroupTask>> CreateGroupTask(GroupTask group);
        public Task<BaseResponse<GroupTask>> UpdateGroupTask(GroupTask group);
        public Task<BaseResponse<bool>> DeleteGroupTask(Guid id);
        public BaseResponse<IQueryable<GroupTask>> GetGroupTaskOData();
    }
}
