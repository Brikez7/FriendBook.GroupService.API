using FriendBook.GroupService.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendBook.GroupService.API.DAL.Configuration
{
    public class AccountStatusInGroupConfig : IEntityTypeConfiguration<AccountStatusInGroup>
    {
        public void Configure(EntityTypeBuilder<AccountStatusInGroup> builder)
        {
            throw new NotImplementedException();
        }
    }
}
