using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FriendBook.GroupService.API.Domain.Entities.MongoDB
{
    public class StageGroupTask
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? Id { get; set; }
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid IdGroupTask { get; set; }
        public string Name { get; set; } = null!;
        public string Text { get; set; } = string.Empty;
        public DateTime DateUpdate { get; set; }
    }
}
