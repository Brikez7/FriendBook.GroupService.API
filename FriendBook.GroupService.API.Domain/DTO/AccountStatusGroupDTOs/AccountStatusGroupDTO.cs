using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.Entities.Postgres.AccountStatusGroup;

namespace FriendBook.GroupService.API.Domain.Entities
{
    public class AccountStatusGroupDTO
    {
        public AccountStatusGroupDTO()
        {
        }

        public AccountStatusGroupDTO(Guid groupId, Guid accountId, RoleAccount roleAccount)
        {
            IdGroup = groupId;
            AccountId = accountId;
            RoleAccount = roleAccount;
        }

        public AccountStatusGroupDTO(AccountStatusGroup createdAccountaStatusGroup)
        {
            IdGroup = createdAccountaStatusGroup.IdGroup;
            AccountId = createdAccountaStatusGroup.AccountId;
            RoleAccount = createdAccountaStatusGroup.RoleAccount;
        }

        public Guid IdGroup { get; set; }
        public Guid AccountId { get; set; }
        public RoleAccount RoleAccount { get; set; } = RoleAccount.Default;
    }
}
