using FriendBook.GroupService.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FriendBook.GroupService.API.DAL
{
    public partial class GroupAppDBContext : DbContext
    {
        public const string NameConnection = "NpgConnectionString";
        public DbSet<Group> Groups { get; set; }
        public DbSet<AccountStatusGroup> AccountsStatusGroups { get; set; }
        public DbSet<GroupTask> GroupTasks { get; set; }
        public async Task UpdateDatabase()
        {
            await Database.EnsureDeletedAsync();
            await Database.MigrateAsync();
        }
        public GroupAppDBContext(DbContextOptions<GroupAppDBContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}