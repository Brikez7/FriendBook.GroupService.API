using FriendBook.GroupService.API.DAL.Configuration.DataType;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FriendBook.GroupService.API.DAL.Configuration
{
    public class GroupTaskConfig : IEntityTypeConfiguration<GroupTask>
    {
        public const string Table_name = "group_tasks";
        public void Configure(EntityTypeBuilder<GroupTask> builder)
        {
            builder.ToTable(Table_name);

            builder.HasKey(e => new { e.Id });

            builder.HasIndex(e => e.GroupId);

            builder.HasIndex(e => new { e.Name,e.GroupId })
                   .IsUnique();

            builder.Property(e => e.Id)
                   .HasColumnType(EntityDataTypes.Guid)
                   .HasColumnName("pk_group_task_id");

            builder.Property(e => e.GroupId)
                   .HasColumnType(EntityDataTypes.Guid)
                   .HasColumnName("group_id");

            builder.Property(e => e.CreaterId)
                   .HasColumnType(EntityDataTypes.Guid)
                   .HasColumnName("creater_id");

            builder.Property(e => e.Name)
                   .HasColumnType(EntityDataTypes.Character_varying)
                   .HasColumnName("name");

            builder.Property(e => e.Description)
                   .HasColumnName(EntityDataTypes.Character_varying)
                   .HasColumnName("description");

            builder.Property(e => e.Status)
                   .HasColumnType(EntityDataTypes.Smallint)
                   .HasColumnName("status_task");

            builder.Property(e => e.DateStartWork)
                   .HasColumnName("date_start_work");

            builder.Property(e => e.DateEndWork)
                   .HasColumnName("date_end_work");

            builder.Property(e => e.Team)
                   .HasColumnName("users_id_in_tasks")
                   .HasColumnType(EntityDataTypes.ArrayGuid);

            builder.HasOne(d => d.Group)
                   .WithMany(p => p.GroupTasks)
                   .HasPrincipalKey(p => p.Id)
                   .HasForeignKey(d => d.GroupId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
