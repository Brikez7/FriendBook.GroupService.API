using MongoDB.Bson;

namespace FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs
{
    public class ResponseStageGroupTaskIcon
    {
        public ObjectId StageGroupTaskId { get; set; }
        public string Name { get; set; }
        public Guid IdGroupTask { get; set; }

        public ResponseStageGroupTaskIcon(ObjectId id, string name, Guid groupId)
        {
            StageGroupTaskId = id;
            Name = name;
            IdGroupTask = groupId;
        }
    }
}
