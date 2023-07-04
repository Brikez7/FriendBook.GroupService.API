namespace FriendBook.GroupService.API.Domain.Entities.Postgres
{
    public class AccountStatusGroup
    {
        public Guid? Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid IdGroup { get; set; }
        public RoleAccount RoleAccount { get; set; } = RoleAccount.Default;

        public AccountStatusGroup(Guid accountId, Guid idGroup, RoleAccount roleAccount)
        {
            AccountId = accountId;
            IdGroup = idGroup;
            RoleAccount = roleAccount;
        }

        public AccountStatusGroup(Guid id)
        {
            Id = id;
        }

        public AccountStatusGroup(AccountStatusGroupDTO accountStatusGroupDTO)
        {
            AccountId = accountStatusGroupDTO.AccountId;
            IdGroup = accountStatusGroupDTO.GroupId;
            RoleAccount = accountStatusGroupDTO.RoleAccount;
        }

        public AccountStatusGroup()
        {
        }

        public Group? Group { get; set; }
    }
    public enum RoleAccount
    {
        Default = 1,
        Admin = 2,
        Creater = 3,
    }
}
