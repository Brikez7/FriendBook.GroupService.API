using Testcontainers.MongoDb;

namespace FriendBook.GroupService.Tests.IntegrationTests.WebAppFactories.ContainerBuilders
{
    internal static class ContainerBuilderMongoDB
    {
        public const string User = "TestMongoDB";
        public const string Password = "TestMongoDB54321!";
        public const string ExposedPort = "27017";
        public const string PortBinding = "27016";
        public const string Image = "mongo:latest";
        public static MongoDbContainer CreateMongoDBContainer() 
        {
            var dbBuilderMongoDB = new MongoDbBuilder();

            return dbBuilderMongoDB
                .WithName($"MongoDB.GroupService.{Guid.NewGuid():N}")
                .WithImage(Image)
                .WithHostname($"MongoHost.GroupService.{Guid.NewGuid():N}")
                .WithExposedPort(ExposedPort)
                .WithPortBinding(PortBinding, true)
                .WithUsername(User)
                .WithPassword(Password)
                .WithCleanUp(true)
                .Build();
        }
    }
}
