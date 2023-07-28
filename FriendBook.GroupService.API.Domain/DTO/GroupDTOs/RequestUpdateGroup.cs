using FriendBook.GroupService.API.Domain.Entities.Postgres;

namespace FriendBook.GroupService.API.Domain.DTO.GroupDTOs
{
    public class RequestUpdateGroup
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; } = null!;

        public RequestUpdateGroup()
        {
        }

        public RequestUpdateGroup(Guid groupId, string name)
        {
            GroupId = groupId;
            Name = name;
        }

        public RequestUpdateGroup(Group createdGroup)
        {
            GroupId = (Guid)createdGroup.Id!;
            Name = createdGroup.Name;
        }
    }
}
