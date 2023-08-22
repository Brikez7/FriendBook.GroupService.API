using NodaTime;

namespace FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs
{
    public class RequestStageGroupTasNew
    {
        public string Name { get; set; }
        public Guid IdGroupTask { get; set; }
        public DateTimeOffset DateCreate { get; set; }
        public RequestStageGroupTasNew(string name, Guid groupId, DateTimeOffset dateCreate)
        {
            Name = name;
            IdGroupTask = groupId;
            DateCreate = dateCreate;
        }
    }
}
