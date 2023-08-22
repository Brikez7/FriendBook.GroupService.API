using FriendBook.GroupService.API.Domain.Entities.Postgres;

namespace FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs
{
    public class ResponseAccountStatusGroupView
    {
        public Guid Id { get; set; }
        public RoleAccount RoleAccount { get; set; }
        public ResponseAccountStatusGroupView(){}

        public ResponseAccountStatusGroupView(Guid id, RoleAccount role)
        {
            Id = id;
            RoleAccount = role;
        }

        public ResponseAccountStatusGroupView(AccountStatusGroup createdAccountStatusGroup)
        {
            Id = (Guid)createdAccountStatusGroup.Id!;
            RoleAccount = createdAccountStatusGroup.RoleAccount;
        }
    }
}