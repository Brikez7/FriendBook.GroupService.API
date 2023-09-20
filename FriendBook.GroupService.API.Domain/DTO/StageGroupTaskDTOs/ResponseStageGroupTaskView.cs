using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NodaTime;

namespace FriendBook.GroupService.API.Domain.DTO.StageGroupTaskDTOs
{
    public class ResponseStageGroupTaskView
    {
        public ObjectId StageId { get; set; }
        public Guid IdGroupTask { get; set; }
        public string Name { get; set; } = null!;
        public string Text { get; set; } = string.Empty;
        public OffsetDateTime DateUpdate { get; set; }
        public DateTime DateCreate { get; set; }

        public ResponseStageGroupTaskView(ObjectId stageId, Guid idGroupTask, string name, string text, OffsetDateTime dateUpdate, DateTime dateCreate)
        {
            StageId = stageId;
            IdGroupTask = idGroupTask;
            Name = name;
            Text = text;
            DateUpdate = dateUpdate;
            DateCreate = dateCreate;
        }

        
    }
}
