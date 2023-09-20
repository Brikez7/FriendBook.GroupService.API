using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs
{
    public class RequestNewStageGroupTask
    {
        public Guid IdGroupTask { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTimeOffset DateCreate { get; set; }
        public RequestNewStageGroupTask(Guid idGroupTask, string name, string text,  DateTimeOffset dateCreate)
        {
            Name = name;
            IdGroupTask = idGroupTask;
            DateCreate = dateCreate;
            Text = text;
        }
    }
}
