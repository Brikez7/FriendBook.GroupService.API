using FriendBook.GroupService.API.Domain.Entities;

namespace FriendBook.GroupService.API.Domain.DTO
{
    public class GroupTaskDTO
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public StatusTask StatusTask { get; set; }
        public DateTime TaskEnd { get; set; }
        public DateTime TaskStart { get; set; }

    }
}
