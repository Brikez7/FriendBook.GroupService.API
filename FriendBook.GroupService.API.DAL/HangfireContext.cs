using Microsoft.EntityFrameworkCore;

namespace FriendBook.GroupService.API.DAL
{
    public class HangfireContext : DbContext
    {
        public const string NameConnection = "HangfireNpgConnectionString";
        public HangfireContext(DbContextOptions<HangfireContext> options) : base(options) { }
    }
}
