using FriendBook.GroupService.API.Domain.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;
using Hangfire;
using Hangfire.PostgreSql;
using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.BLL.GrpcServices;
using FriendBook.GroupService.Tests.WebAppFactories.ContainerBuilders;
using Npgsql;

namespace FriendBook.GroupService.Tests.WebAppFactories
{
    internal class WebHostFactory<TProgram, TDbContext> : WebApplicationFactory<TProgram>
        where TProgram : class where TDbContext : DbContext
    {
        private readonly PostgreSqlContainer _postgresContainer;
        private readonly MongoDbContainer _mongoDbContainer;
        internal TestGrpcClient DecoratorGrpcClient;
        internal WebHostFactory()
        {
            _postgresContainer = ContainerBuilderPostgres.CreatePostgreSQLContainer();
            _mongoDbContainer = ContainerBuilderMongoDB.CreateMongoDBContainer();
            DecoratorGrpcClient = new TestGrpcClient();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.Test.json");

            builder.ConfigureAppConfiguration((conf) => conf.AddJsonFile(configPath));

            builder.ConfigureTestServices(services =>
            {
                services.ReplaceDbContext<TDbContext>(_postgresContainer.GetConnectionString());
                services.ReplaceConnectionHangfire(_postgresContainer.GetConnectionString());
                services.ReplaceConnectionMongoDB(_mongoDbContainer.GetConnectionString());
                services.ReplaceGrpcService(DecoratorGrpcClient);
            });

            builder.UseEnvironment("Test");
        }
        internal async Task InitializeAsync()
        {
            var task1 = _postgresContainer.StartAsync();
            var task2 = _mongoDbContainer.StartAsync();

            await Task.WhenAll(task1, task2);
        }
        internal async Task ClearData()
        {
            var dbPostges = Services.GetRequiredService<TDbContext>();
            var repStage = Services.GetRequiredService<IStageGroupTaskRepository>();

            var taskClearPostgres = dbPostges.Database.EnsureDeletedAsync();
            var taskClearStageTasks = repStage.Delete(x => true);

            await Task.WhenAll(taskClearStageTasks, taskClearPostgres);

            await dbPostges.Database.MigrateAsync();
        }
        public override async ValueTask DisposeAsync()
        {
            await _postgresContainer.DisposeAsync();
            await _mongoDbContainer.DisposeAsync();

            await base.DisposeAsync();

            GC.SuppressFinalize(this);
        }
    }
    internal static class ServiceCollectionExtensions
    {
        internal static void ReplaceDbContext<T>(this IServiceCollection services, string newConnectionPostgres) where T : DbContext
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));
            if (descriptor != null) services.Remove(descriptor);

            var connectionMainPostgres = newConnectionPostgres.Replace("Database=postgres", $"Database={ContainerBuilderPostgres.Database}");

            services.AddDbContext<T>(options => { options.UseNpgsql(connectionMainPostgres,o => o.UseNodaTime()); });
        }
        internal static void ReplaceGrpcService(this IServiceCollection services, IGrpcClient testGrpcClient)
        {
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IGrpcClient));
            if (descriptor != null) services.Remove(descriptor);

            services.AddSingleton(testGrpcClient);
        }
        internal static void ReplaceConnectionMongoDB(this IServiceCollection services, string newConnectionMongoDB)
        {
            var existingOptions = services.BuildServiceProvider().GetRequiredService<IOptions<MongoDBSettings>>();
            MongoDBSettings value = existingOptions.Value;
            value.MongoDBConnectionString = newConnectionMongoDB;

            var updatedOptions = Options.Create(value);
            services.AddSingleton(updatedOptions);
        }
        internal static void ReplaceConnectionHangfire(this IServiceCollection services, string newConnectionPostgres)
        {
            var connectionHangfirePostgres = newConnectionPostgres.Replace("Database=postgres", $"Database={ContainerBuilderPostgres.DatabaseHangfire}");

            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(JobStorage));
            if (descriptor != null) services.Remove(descriptor);

            services.AddScoped<JobStorage>(x =>
            {
                var jobStorage = new PostgreSqlStorage(connectionHangfirePostgres);
                return jobStorage;
            });

            services.AddHangfire((configuration) =>
            {
                configuration.UseSimpleAssemblyNameTypeSerializer()
                             .UseRecommendedSerializerSettings()
                             .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                             .UsePostgreSqlStorage(connectionHangfirePostgres);
            });

            services.AddHangfireServer();
        }
    }
}
