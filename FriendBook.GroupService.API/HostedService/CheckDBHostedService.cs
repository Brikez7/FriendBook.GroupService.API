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
        public static async void CreateUniqueIndex(IMongoCollection<StageGroupTask> collection)
        {
            var indexKeys = Builders<StageGroupTask>.IndexKeys.Combine(
                Builders<StageGroupTask>.IndexKeys.Ascending(x => x.IdGroupTask),
                Builders<StageGroupTask>.IndexKeys.Ascending(x => x.Name));

            var indexModel = new CreateIndexModel<StageGroupTask>(indexKeys, new CreateIndexOptions { Name = "Index_StageName_TaskId", Unique = true });

            var existingIndexes = (await collection.Indexes.ListAsync()).ToList();

            bool indexExists = existingIndexes.Any(i => i["name"] == indexModel.Options.Name);

            if (!indexExists)
            {
                await collection.Indexes.CreateOneAsync(indexModel);
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool collectionExists = await (await _mongoDatabase.ListCollectionNamesAsync()).AnyAsync();
            if (!collectionExists)
            {
                await _mongoDatabase.CreateCollectionAsync(_settings.Collection);
            }

            var collection = _mongoDatabase.GetCollection<StageGroupTask>(_settings.Collection);
            CreateUniqueIndex(collection);

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