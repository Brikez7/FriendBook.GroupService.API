using FriendBook.GroupService.Tests.IntegrationTests.WebAppFactories.ContainerBuilders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace FriendBook.GroupService.Tests.IntegrationTests.WebAppFactories
{
    internal class WebHostFactory<TProgram, TDbContext> : WebApplicationFactory<TProgram>
        where TProgram : class where TDbContext : DbContext
    {
        private readonly PostgreSqlContainer _postgresContainer;
        public WebHostFactory()
        {
            _postgresContainer = ContainerBuilderPostgres.CreatePostgreSQLContainer();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.Test.json");

            builder.ConfigureAppConfiguration((conf) => conf.AddJsonFile(configPath));

            builder.ConfigureTestServices(services =>
            {
                services.ReplaceDbContext<TDbContext>(_postgresContainer.GetConnectionString());
            });

            builder.UseEnvironment("Test");
        }
        internal async Task InitializeAsync()
        {
            var task1 = _postgresContainer.StartAsync();

            await Task.WhenAll(task1);
        }
        internal async Task ClearData()
        {
            var dbPostges = Services.GetRequiredService<TDbContext>();

            var task2 = dbPostges.Database.EnsureDeletedAsync();

            await Task.WhenAll(task2);

            await dbPostges.Database.MigrateAsync();
        }
        public override async ValueTask DisposeAsync()
        {
            await _postgresContainer.DisposeAsync();

            await base.DisposeAsync();

            GC.SuppressFinalize(this);
        }
    }
    internal static class ServiceCollectionExtensions
    {
        public static void ReplaceDbContext<T>(this IServiceCollection services, string newConnectionPostgres) where T : DbContext
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));
            if (descriptor != null) services.Remove(descriptor);

            Console.WriteLine(newConnectionPostgres);
            services.AddDbContext<T>(options => { options.UseNpgsql(newConnectionPostgres); });
        }
    }
}
