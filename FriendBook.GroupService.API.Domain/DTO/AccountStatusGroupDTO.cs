using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendBook.GroupService.API.Domain.Entities;

namespace FriendBook.GroupService.API.Domain.DTO
{
    public class AccountStatusGroupDTO
    {
        public Guid GroupId { get; set; }
        public Guid AccountId { get; set; }
        public StatusAccount RoleAccount { get; set; } = StatusAccount.Default;
    }
}
