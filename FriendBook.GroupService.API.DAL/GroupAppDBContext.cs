using FriendBook.GroupService.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FriendBook.GroupService.API.DAL
{
    public class GroupAppDBContext : DbContext
    {
        public const string NameConnection = "NpgConnectionString";
        public DbSet<Group> Groups { get; set; }
        public DbSet<AccountStatusGroup> AccountsStatusGroups { get; set; }

        public void UpdateDatabase()
        {
            Database.EnsureDeleted();
            Database.Migrate();
        }
        public GroupAppDBContext(DbContextOptions<GroupAppDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}