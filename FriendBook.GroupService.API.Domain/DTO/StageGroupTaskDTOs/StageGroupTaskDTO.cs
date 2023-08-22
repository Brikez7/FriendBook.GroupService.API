using MongoDB.Bson;
using NodaTime;

namespace FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs
{
    public class StageGroupTaskDTO
    {
        public ObjectId StageId { get; set; }
        public Guid IdGroupTask { get; set; }
        public string Name { get; set; } = null!;
        public string Text { get; set; } = string.Empty;
        public OffsetDateTime DateUpdate { get; set; }
        public DateTime DateCreate { get; set; }

        public StageGroupTaskDTO(ObjectId stageId, Guid idGroupTask, string name, string text, OffsetDateTime dateUpdate, DateTime dateCreate)
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
