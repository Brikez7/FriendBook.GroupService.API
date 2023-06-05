namespace FriendBook.GroupService.API.Domain.DTO.GroupTasksDTO
{
    public class GroupTaskKeyDTO
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; } = null!;

        public GroupTaskKeyDTO()
        {
        }

        public GroupTaskKeyDTO(Guid groupId, string name)
        {
            GroupId = groupId;
            Name = name;
        }
    }
}
