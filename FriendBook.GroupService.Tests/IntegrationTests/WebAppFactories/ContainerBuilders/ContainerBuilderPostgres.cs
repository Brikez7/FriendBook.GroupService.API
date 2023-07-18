using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;

namespace FriendBook.GroupService.Tests.IntegrationTests.WebAppFactories.ContainerBuilders
{
    internal class ContainerBuilderPostgres
    {
        public const string User = "TestPostgres";
        public const string Password = "TestPostgres54321!";
        public const string Database = "test_friendbook_groupservice";
        public const string DatabaseHangfire = "test_friendbook_hangfire";
        public const string ExposedPort = "5433";
        public const string PortBinding = "5432";
        public const string Image = "postgres:latest";
        public static PostgreSqlContainer CreatePostgreSQLContainer()
        {
            var dbBuilderPostgre = new PostgreSqlBuilder();

            return dbBuilderPostgre
                .WithName($"PostgresDB.GroupService.{Guid.NewGuid():N}")
                .WithImage(Image)
                .WithHostname($"PostgresHost.GroupService.{Guid.NewGuid():N}")
                .WithExposedPort(ExposedPort)
                .WithPortBinding(PortBinding, true)
                .WithUsername(User)
                .WithPassword(Password)
                .WithTmpfsMount("/pgdata")
                .WithCleanUp(true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted($"psql -U {User} -d postgres -c 'CREATE DATABASE {DatabaseHangfire}';" +
                                                                                  $"export PGPASSWORD='{Password}';psql -U {User} -d '{DatabaseHangfire}' -c \"select 1\";" +
                                                                                  $"psql -U {User} -d postgres -c 'CREATE DATABASE {Database}';" +
                                                                                  $"export PGPASSWORD='{Password}';psql -U {User} -d '{Database}' -c \"select 1\";"))
                .Build();
        }
    }
}
