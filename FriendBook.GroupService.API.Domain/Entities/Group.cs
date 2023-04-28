using FriendBook.GroupService.API.Domain.DTO;

namespace FriendBook.GroupService.API.Domain.Entities
{
    public class Group
    {
        public Guid? Id { get; set; }
        public Guid CreatedId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedDate { get; set; }

        public Group()
        {
        }

        public Group(GroupDTO groupDTO, Guid accountId)
        {
            Id = groupDTO.GroupId;
            Name = groupDTO.Name;
            CreatedId = accountId;
            CreatedDate = DateTime.Now;
        }

        public Group(Guid? id)
        {
            Id = id;
        }

        public IEnumerable<AccountStatusGroup>? AccountStatusGroups { get; set; }
    }
}