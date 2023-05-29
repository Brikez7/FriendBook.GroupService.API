using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendBook.GroupService.API.Domain.Entities;

namespace FriendBook.GroupService.API.Domain
{
    public class AccountStatusGroupDTO
    {
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
            AccountId= createdAccountaStatusGroup.AccountId;
            RoleAccount = createdAccountaStatusGroup.RoleAccount;
        }

        public Guid GroupId { get; set; }
        public Guid AccountId { get; set; }
        public RoleAccount RoleAccount { get; set; } = RoleAccount.Default;
    }
}
