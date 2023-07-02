using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.Response;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IContactGroupService
    {
        public Task<BaseResponse<GroupDTO>> CreateGroup(string groupName, Guid createrId);
        public Task<BaseResponse<GroupDTO>> UpdateGroup(GroupDTO group, Guid userId);
        public Task<BaseResponse<bool>> DeleteGroup(Guid id, Guid userId);
        public BaseResponse<IQueryable<Group>> GetGroupOData();
        public Task<BaseResponse<GroupDTO[]>> GeyGroupsByUserId(Guid userId);
        public Task<BaseResponse<ResponseAccountGroup[]>> GetGroupsWithStatusByUserId(Guid userId);
    }
}