using MongoDB.Bson;

namespace FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs
{
    public class StageGroupTaskDTO
    {
        public ObjectId StageGroupTaskId { get; set; }
        public Guid IdGroupTask { get; set; }
        public string Name { get; set; } = null!;
        public string Text { get; set; } = string.Empty;
        public DateTimeOffset DateUpdate { get; set; }
        public DateTimeOffset DateCreate { get; set; }

        public StageGroupTaskDTO(Guid idGroupTask, string name, string text, DateTimeOffset dateUpdate, DateTimeOffset dateCreate)
        {
            IdGroupTask = idGroupTask;
            Name = name;
            Text = text;
            DateUpdate = dateUpdate;
            DateCreate = dateCreate;
        }
    }
}
