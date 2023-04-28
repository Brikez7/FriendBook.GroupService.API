using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendBook.GroupService.API.Domain.DTO
{
    public class AccountStatusGroupDTO
    {
        public Guid GroupId { get; set; }
        public Guid AccountId { get; set; }
        public RoleAccount RoleAccount { get; set; } = RoleAccount.Default;
    }
}
