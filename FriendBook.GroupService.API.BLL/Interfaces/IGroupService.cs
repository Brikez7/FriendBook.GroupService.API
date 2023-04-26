using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IGroupService
    {
        public Task<BaseResponse<Group>> CreateGroup(Group comment);
        public Task<BaseResponse<bool>> DeleteGroup(Guid id);
        public BaseResponse<IQueryable<Group>> GetGroupOData();
    }
}