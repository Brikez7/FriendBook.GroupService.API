using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FriendBook.GroupService.API.Domain.Entities.MongoDB
{
    public class StageGroupTask
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid IdGroupTask { get; set; }
        public string Name { get; set; } = null!;
        public string Text { get; set; } = string.Empty;
        public DateTime DateUpdate { get; set; }
        public DateTime? DateCreate { get; set; }

        public StageGroupTask(Guid idGroupTask, string name, string text, DateTime dateUpdate, DateTime dateCreate)
        {
            Id = ObjectId.GenerateNewId();
            IdGroupTask = idGroupTask;
            Name = name;
            Text = text;
            DateUpdate = dateUpdate;
            DateCreate = dateCreate;
        }

        public StageGroupTask(ObjectId id, Guid idGroupTask, string name, string text, DateTime dateUpdate)
        {
            Id = id;
            IdGroupTask = idGroupTask;
            Name = name;
            Text = text;
            DateUpdate = dateUpdate;
        }
    }
}
