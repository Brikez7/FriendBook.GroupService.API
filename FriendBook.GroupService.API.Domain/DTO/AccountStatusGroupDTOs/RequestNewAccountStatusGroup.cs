using FriendBook.GroupService.API.Domain.Entities.Postgres;

namespace FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs
{
    public class RequestNewAccountStatusGroup
    {
        public Guid GroupId { get; set; }
        public Guid AccountId { get; set; }
        public RoleAccount RoleAccount { get; set; } = RoleAccount.Default;
        public RequestNewAccountStatusGroup()
        {
        }

        public RequestNewAccountStatusGroup(Guid groupId, Guid accountId, RoleAccount roleAccount)
        {
            GroupId = groupId;
            AccountId = accountId;
            RoleAccount = roleAccount;
        }

        public RequestNewAccountStatusGroup(AccountStatusGroup createdAccountStatusGroup)
        {
            GroupId = createdAccountStatusGroup.IdGroup;
            AccountId = createdAccountStatusGroup.AccountId;
            RoleAccount = createdAccountStatusGroup.RoleAccount;
        }
    }
}
