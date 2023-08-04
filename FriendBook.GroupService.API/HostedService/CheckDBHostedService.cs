using FriendBook.GroupService.API.DAL;
using FriendBook.GroupService.API.Domain.Entities.MongoDB;
using FriendBook.GroupService.API.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Npgsql;

namespace FriendBook.GroupService.API.BackgroundHostedService
{
    public class CheckDBHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly MongoDBSettings _settings;
        private readonly IConfiguration _configuration;

        public CheckDBHostedService(IServiceScopeFactory serviceScopeFactory, IMongoDatabase mongoDatabase, IOptions<MongoDBSettings> options, IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mongoDatabase = mongoDatabase;
            _settings = options.Value;
            _configuration = configuration;
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
                await collection.Indexes.CreateOneAsync(indexModel);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            bool collectionExists = await(await _mongoDatabase.ListCollectionNamesAsync(cancellationToken: cancellationToken)).AnyAsync(cancellationToken);
            if (!collectionExists)
                await _mongoDatabase.CreateCollectionAsync(_settings.Collection, cancellationToken: cancellationToken);

            var collection = _mongoDatabase.GetCollection<StageGroupTask>(_settings.Collection);
            CreateUniqueIndex(collection);

            using var scope = _serviceScopeFactory.CreateScope();
            var appDbContext = scope.ServiceProvider.GetRequiredService<GroupDBContext>();

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(appDbContext.Database.GetConnectionString());
            dataSourceBuilder.UseNodaTime();
            dataSourceBuilder.Build();

            if (!await appDbContext.Database.CanConnectAsync(cancellationToken) || (await appDbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
            {
                await appDbContext.Database.MigrateAsync(cancellationToken);
                return;
            }

            return;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}