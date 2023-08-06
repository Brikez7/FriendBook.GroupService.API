using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.Response;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IGroupService
    {
        public Task<BaseResponse<ResponseGroupView>> CreateGroup(string groupName, Guid creatorId);
        public Task<BaseResponse<ResponseGroupView>> UpdateGroup(RequestUpdateGroup group, Guid creatorId);
        public Task<BaseResponse<bool>> DeleteGroup(Guid groupId, Guid creatorId);
        public BaseResponse<IQueryable<Group>> GetGroupOData();
        public Task<BaseResponse<ResponseGroupView[]>> GetGroupsByCreatorId(Guid userId);
        public Task<BaseResponse<ResponseAccountGroup[]>> GetGroupsWithStatusByUserId(Guid userId);
    }
}