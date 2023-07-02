using FriendBook.GroupService.API.DAL;
using FriendBook.GroupService.API.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FriendBook.GroupService.API.BackgroundHostedService
{
    public class CheckDBHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private GroupAppDBContext? _appDBContext;
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public CheckDBHostedService(IServiceScopeFactory serviceScopeFactory, IOptions<MongoDBSettings> mongoDBSettings)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _client = new MongoClient(mongoDBSettings.Value.MongoDBConnectionString);
            _database = _client.GetDatabase(mongoDBSettings.Value.Database);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {


            using var scope = _serviceScopeFactory.CreateScope();
            _appDBContext = scope.ServiceProvider.GetRequiredService<GroupAppDBContext>();

            if (!await _appDBContext.Database.CanConnectAsync() || (await _appDBContext.Database.GetPendingMigrationsAsync(stoppingToken)).Any())
            {
                await _appDBContext.Database.MigrateAsync(stoppingToken);
                return;
            }
            
            return;
        }
    }
}