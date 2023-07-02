namespace FriendBook.GroupService.API.Domain.Settings
{
    public class MongoDBSettings
    {
        public const string Name = "MongoDBSettings";
        public string MongoDBConnectionString { get; set; } = null!;
        public string Database { get; set; } = null!;
        public string Collection { get; set; } = null!;
    }
}
