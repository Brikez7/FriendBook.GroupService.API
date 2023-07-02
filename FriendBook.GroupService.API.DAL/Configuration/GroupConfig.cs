using FriendBook.GroupService.API.DAL.Configuration.DataType;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FriendBook.GroupService.API.DAL.Configuration
{
    public class GroupConfig : IEntityTypeConfiguration<Group>
    {
        public const string Table_name = "groups";

        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable(Table_name);

            builder.HasKey(e => new { e.Id });

            builder.HasIndex(e => e.CreaterId);

            builder.HasIndex(e => e.Name)
                   .IsUnique();

            builder.Property(e => e.Id)
                   .HasColumnType(EntityDataTypes.Guid)
                   .HasColumnName("pk_group_id");

            builder.Property(e => e.CreaterId)
                   .HasColumnType(EntityDataTypes.Guid)
                   .HasColumnName("account_id");

            builder.Property(e => e.Name)
                   .HasColumnType(EntityDataTypes.Character_varying)
                   .HasColumnName("name");

            builder.Property(e => e.CreatedDate)
                   .HasColumnName("create_date");
        }
    }
}