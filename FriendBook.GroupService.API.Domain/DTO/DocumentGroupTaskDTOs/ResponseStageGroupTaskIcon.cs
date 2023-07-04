using MongoDB.Bson;

namespace FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs
{
    public class StageGroupTaskIconDTO
    {
        public ObjectId StageGroupTaskId { get; set; }
        public string Name { get; set; }
        public Guid IdGroupTask { get; set; }

        public StageGroupTaskIconDTO(ObjectId id, string name, Guid groupId)
        {
            StageGroupTaskId = id;
            Name = name;
            IdGroupTask = groupId;
        }
    }
}
