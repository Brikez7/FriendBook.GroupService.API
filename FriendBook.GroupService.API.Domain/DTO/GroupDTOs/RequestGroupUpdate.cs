using FriendBook.GroupService.API.Domain.Entities.Postgres;

namespace FriendBook.GroupService.API.Domain.DTO.GroupDTOs
{
    public class RequestGroupUpdate
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; } = null!;

        public RequestGroupUpdate()
        {
        }

        public RequestGroupUpdate(Group createdGroup)
        {
            GroupId = (Guid)createdGroup.Id!;
            Name = createdGroup.Name;
        }
    }
}
