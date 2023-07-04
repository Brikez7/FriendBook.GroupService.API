using FriendBook.GroupService.API.Domain.Entities.Postgres;

namespace FriendBook.GroupService.API.Domain.Entities
{
    public class AccountStatusGroupDTO
    {
        public Guid GroupId { get; set; }
        public Guid AccountId { get; set; }
        public RoleAccount RoleAccount { get; set; } = RoleAccount.Default;
        public AccountStatusGroupDTO()
        {
        }

        public AccountStatusGroupDTO(Guid groupId, Guid accountId, RoleAccount roleAccount)
        {
            GroupId = groupId;
            AccountId = accountId;
            RoleAccount = roleAccount;
        }

        public AccountStatusGroupDTO(AccountStatusGroup createdAccountaStatusGroup)
        {
            GroupId = createdAccountaStatusGroup.IdGroup;
            AccountId = createdAccountaStatusGroup.AccountId;
            RoleAccount = createdAccountaStatusGroup.RoleAccount;
        }
    }
}
