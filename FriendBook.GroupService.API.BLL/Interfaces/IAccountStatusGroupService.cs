using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IAccountStatusGroupService
    {
        public Task<BaseResponse<AccountStatusGroup>> CreateAccountStatusGroup(AccountStatusGroup accountStatusGroup);
        public Task<BaseResponse<bool>> DeleteAccountStatusGroup(Guid id);
        public BaseResponse<IQueryable<AccountStatusGroup>> GetAccountStatusGroupOData();
        public Task<BaseResponse<AccountStatusGroup>> UpdateAccountStatusGroup(AccountStatusGroup accountStatusGroup);
    }
}
