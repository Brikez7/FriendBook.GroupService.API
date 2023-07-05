using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.Response;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IContactGroupService
    {
        public Task<BaseResponse<ResponseGroupView>> CreateGroup(string groupName, Guid createrId);
        public Task<BaseResponse<RequestGroupUpdate>> UpdateGroup(RequestGroupUpdate group, Guid createrId);
        public Task<BaseResponse<bool>> DeleteGroup(Guid groupid, Guid createrId);
        public BaseResponse<IQueryable<Group>> GetGroupOData();
        public Task<BaseResponse<ResponseGroupView[]>> GetGroupsByCreaterId(Guid userId);
        public Task<BaseResponse<ResponseAccountGroup[]>> GetGroupsWithStatusByUserId(Guid userId);
    }
}