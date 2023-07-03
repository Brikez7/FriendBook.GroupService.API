using FriendBook.GroupService.API.Domain.Entities.Postgres;

namespace FriendBook.GroupService.API.Domain.Entities
{
    public class AccountStatusGroupDTO
    {
        public Guid IdGroup { get; set; }
        public Guid AccountId { get; set; }
        public RoleAccount RoleAccount { get; set; } = RoleAccount.Default;
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
    }
}
