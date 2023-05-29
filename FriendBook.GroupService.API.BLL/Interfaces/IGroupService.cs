using FriendBook.GroupService.API.Domain.DTO;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IGroupService
    {
        public Task<BaseResponse<GroupDTO>> CreateGroup(Group group);
        public Task<BaseResponse<GroupDTO>> UpdateGroup(Group group);
        public Task<BaseResponse<bool>> DeleteGroup(Guid id);
        public BaseResponse<IQueryable<Group>> GetGroupOData();
    }
}