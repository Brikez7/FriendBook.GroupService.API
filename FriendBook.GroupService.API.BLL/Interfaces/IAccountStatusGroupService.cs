using FriendBook.GroupService.API.BLL.gRPCClients.AccountService;
using FriendBook.GroupService.API.BLL.gRPCClients.ContactService;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.Response;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IAccountStatusGroupService
    {
        public Task<BaseResponse<AccountStatusGroupDTO>> CreateAccountStatusGroup(Guid userId, AccountStatusGroupDTO accountStatusGroup);
        public Task<BaseResponse<bool>> DeleteAccountStatusGroup(Guid id, Guid createrId, Guid groupId);
        public Task<BaseResponse<AccountStatusGroupDTO>> UpdateAccountStatusGroup(AccountStatusGroupDTO accountStatusGroup, Guid idCreater);
        public BaseResponse<IQueryable<AccountStatusGroup>> GetAccountStatusGroupOData();
        public Task<BaseResponse<Profile[]>> GetProfilesByIdGroup(Guid idGroup, ResponseProfiles profileDTOs);
        public Task<BaseResponse<AccountStatusGroup?>> GetAccountStatusesGroupFromUserGroup(Guid userId, Guid groupId);
        public BaseResponse<ResponseTasksPage> TasksJoinUsersLoginWithId(List<GroupTask> groupTasks, User[] usersLoginWithId, bool isAdmin);
    }
}
