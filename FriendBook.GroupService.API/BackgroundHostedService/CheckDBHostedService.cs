using FriendBook.GroupService.API.DAL;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.GroupService.API.BackgroundHostedService
{
    public class CheckDBHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private GroupAppDBContext? _appDBContext;

        public CheckDBHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            _appDBContext = scope.ServiceProvider.GetRequiredService<GroupAppDBContext>();

            if (await _appDBContext.Database.EnsureCreatedAsync())
            {
                await _appDBContext.UpdateDatabase();
            }
            return;
        }
    }
}