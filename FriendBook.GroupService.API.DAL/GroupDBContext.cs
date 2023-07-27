using FriendBook.GroupService.API.Domain.Entities.Postgres;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FriendBook.GroupService.API.DAL
{
    public partial class GroupDBContext : DbContext
    {
        public const string NameConnection = "NpgConnectionString";
        public DbSet<Group> Groups { get; set; }
        public DbSet<AccountStatusGroup> AccountsStatusGroups { get; set; }
        public DbSet<GroupTask> GroupTasks { get; set; }
        public async Task UpdateDatabase()
        {
            await Database.MigrateAsync();
        }
        public GroupDBContext(DbContextOptions<GroupDBContext> options) : base(options)
        {
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