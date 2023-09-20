using MongoDB.Bson;

namespace FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs
{
    public class ResponseStageGroupTaskIcon
    {
        public ObjectId StageGroupTaskId { get; set; } 
        public string Name { get; set; } = null!;

        public ResponseStageGroupTaskIcon(ObjectId id, string name)
        {
            StageGroupTaskId = id;
            Name = name;
        }
    }
}
