using FriendBook.GroupService.API.Domain;
using FriendBook.GroupService.API.Domain.DTO;
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
        public Task<BaseResponse<ProfileDTO[]>> GetProfilesByIdGroup(Guid idGroup, ProfileDTO[] profileDTOs);
        public Task<BaseResponse<AccountStatusGroup?>> GetAccountStatusGroupByIdGroupAndUserId(Guid userId, Guid groupId);
        public BaseResponse<TasksPageDTO> TasksJoinUsersLoginWithId(List<GroupTask> groupTasks, Tuple<Guid, string>[] usersLoginWithId, bool isAdmin);
    }
}
