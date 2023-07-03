using FriendBook.GroupService.API.DAL;
using FriendBook.GroupService.API.Domain.Entities.MongoDB;
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
        private readonly IMongoDatabase _mongoDatabase;
        private readonly MongoDBSettings _settings;

        public CheckDBHostedService(IServiceScopeFactory serviceScopeFactory, IMongoDatabase mongoDatabase, IOptions<MongoDBSettings> options)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mongoDatabase = mongoDatabase;
            _settings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool collectionExists = await (await _mongoDatabase.ListCollectionNamesAsync()).AnyAsync();
            if (!collectionExists)
            {
                await _mongoDatabase.CreateCollectionAsync(_settings.Collection);
            }

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