namespace FriendBook.GroupService.API.Domain.Entities
{
    public class AccountStatusGroup 
    {
        public Guid? Id { get; set; }
        public Guid IdAccount { get; set; }
        public Guid IdGroup { get; set; }
        public RoleAccount RoleAccount { get; set; } = RoleAccount.Default;
        public AccountStatusGroup(Guid? id, Guid idAccount, Guid idGroup, RoleAccount roleAccount)
        {
            Id = id;
            IdAccount = idAccount;
            IdGroup = idGroup;
            RoleAccount = roleAccount;
        }

        public Group? Group { get; set; }
    }
}
