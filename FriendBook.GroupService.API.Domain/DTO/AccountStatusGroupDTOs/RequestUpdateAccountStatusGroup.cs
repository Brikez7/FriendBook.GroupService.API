using FriendBook.GroupService.API.Domain.Entities.Postgres;

namespace FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs
{
    public class RequestUpdateAccountStatusGroup
    {
        public Guid Id { get; set; }
        public RoleAccount RoleAccount { get; set; }

        public RequestUpdateAccountStatusGroup()
        {
        }

        public RequestUpdateAccountStatusGroup(Guid id, RoleAccount roleAccount)
        {
            Id = id;
            RoleAccount = roleAccount;
        }
    }
}
