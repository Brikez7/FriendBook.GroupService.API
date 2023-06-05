namespace FriendBook.GroupService.API.Domain.DTO.GroupTasksDTO
{
    public class GroupTaskNewDTO
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public DateTime DateEndWork { get; set; }

        public GroupTaskNewDTO(Guid groupId, string name, string description, DateTime dateEndWork)
        {
            GroupId = groupId;
            Name = name;
            Description = description;
            DateEndWork = dateEndWork;
        }

        public GroupTaskNewDTO()
        {
        }
    }
}
