using FriendBook.GroupService.API.Domain.DTO;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IGroupService
    {
        public Task<BaseResponse<GroupDTO>> CreateGroup(Group group);
        public Task<BaseResponse<GroupDTO>> UpdateGroup(GroupDTO group, Guid userId);
        public Task<BaseResponse<bool>> DeleteGroup(Guid id, Guid userId);
        public BaseResponse<IQueryable<Group>> GetGroupOData();
        public Task<BaseResponse<GroupDTO[]>> GeyGroupsByUserId(Guid userId);
        public Task<BaseResponse<AccountGroupDTO[]>> GetGroupsWithStatusByUserId(Guid userId);
    }
}