using FriendBook.GroupService.API.DAL.Configuration.DataType;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FriendBook.GroupService.API.DAL.Configuration
{
    public class AccountStatusGroupConfig : IEntityTypeConfiguration<AccountStatusGroup>
    {
        public const string Table_name = " account_status_groups";
        public void Configure(EntityTypeBuilder<AccountStatusGroup> builder)
        {
            builder.ToTable(Table_name);

            builder.HasKey(e => new { e.Id });

            builder.HasIndex(e => e.IdGroup);

            builder.Property(e => e.Id)
                   .HasColumnType(EntityDataTypes.Guid)
                   .HasColumnName("pk_account_status_groups_id");

            builder.Property(e => e.AccountId)
                   .HasColumnType(EntityDataTypes.Guid)
                   .HasColumnName("account_id");

            builder.Property(e => e.IdGroup)
                   .HasColumnName("group_id")
                   .HasColumnType(EntityDataTypes.Guid);

            builder.Property(e => e.RoleAccount)
                   .HasColumnName("role_account")
                   .HasColumnType(EntityDataTypes.Smallint);

            builder.HasOne(d => d.Group)
                   .WithMany(p => p.AccountStatusGroups)
                   .HasPrincipalKey(p => p.Id)
                   .HasForeignKey(d => d.IdGroup);
        }
    }
}
