using FriendBook.GroupService.API.BLL.gRPCServices.AccountService;
using FriendBook.GroupService.API.BLL.gRPCServices.ContactService;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.Response;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IAccountStatusGroupService
    {
        public Task<BaseResponse<AccountStatusGroupDTO>> CreateAccountStatusGroup(Guid createrId, AccountStatusGroupDTO accountStatusGroupDTO);
        public Task<BaseResponse<bool>> DeleteAccountStatusGroup(Guid accountStatusGroupId, Guid createrId, Guid groupId);
        public Task<BaseResponse<AccountStatusGroupDTO>> UpdateAccountStatusGroup(AccountStatusGroupDTO accountStatusGroup, Guid createrId);
        public BaseResponse<IQueryable<AccountStatusGroup>> GetAccountStatusGroupOData();
        public Task<BaseResponse<Profile[]>> GetProfilesByIdGroup(Guid groupId, ResponseProfiles responseProfiles);
        public Task<BaseResponse<AccountStatusGroup?>> GetAccountStatusesGroupFromUserGroup(Guid userId, Guid groupId);
        public Task<BaseResponse<ResponseTasksPage>> GetTasksPage(List<GroupTask> groupTasks, User[] users, bool isAdmin);
    }
}
