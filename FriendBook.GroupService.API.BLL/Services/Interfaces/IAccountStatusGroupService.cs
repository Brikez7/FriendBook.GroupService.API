using FriendBook.GroupService.API.BLL.gRPCClients.ContactClient;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.Response;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IAccountStatusGroupService
    {
        public Task<BaseResponse<ResponseAccountStatusGroupView>> CreateAccountStatusGroup(Guid creatorId, RequestNewAccountStatusGroup requestNewAccountStatusGroup);
        public Task<BaseResponse<ResponseAccountStatusGroupView>> UpdateAccountStatusGroup(RequestUpdateAccountStatusGroup accountStatusGroup, Guid creatorId);
        public Task<BaseResponse<Profile[]>> GetProfilesByIdGroup(Guid groupId, ResponseProfiles responseProfiles);
        public Task<BaseResponse<bool>> DeleteAccountStatusGroup(Guid deletedAccountStatusGroupId, Guid creatorId);
    }
}
