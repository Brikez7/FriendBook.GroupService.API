namespace FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs
{
    public class RequestGroupTaskKey
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; } = null!;

        public RequestGroupTaskKey()
        {
        }

        public RequestGroupTaskKey(Guid groupId, string name)
        {
            GroupId = groupId;
            Name = name;
        }
    }
}
