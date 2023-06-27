using FriendBook.GroupService.API.BLL.gRPCClients.AccountService;
using FriendBook.GroupService.API.BLL.gRPCClients.ContactService;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IAccountStatusGroupService
    {
        public Task<BaseResponse<AccountStatusGroupDTO>> CreateAccountStatusGroup(AccountStatusGroupDTO accountStatusGroup);
        public Task<BaseResponse<bool>> DeleteAccountStatusGroup(Guid id, Guid createrId, Guid groupId);
        public Task<BaseResponse<AccountStatusGroupDTO>> UpdateAccountStatusGroup(AccountStatusGroupDTO accountStatusGroup, Guid idCreater);
        public BaseResponse<IQueryable<AccountStatusGroup>> GetAccountStatusGroupOData();
        public Task<BaseResponse<Profile[]>> GetProfilesByIdGroup(Guid idGroup, ResponseProfiles profileDTOs);
        public Task<BaseResponse<AccountStatusGroup?>> GetAccountStatusGroupByIdGroupAndUserId(Guid userId, Guid groupId);
        public BaseResponse<ResponseTasksPage> TasksJoinUsersLoginWithId(List<GroupTask> groupTasks, User[] usersLoginWithId, bool isAdmin);
    }
}
