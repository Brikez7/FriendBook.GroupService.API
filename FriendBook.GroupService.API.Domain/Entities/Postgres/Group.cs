using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;

namespace FriendBook.GroupService.API.Domain.Entities.Postgres
{
    public class Group
    {
        public Guid? Id { get; set; }
        public Guid CreaterId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedDate { get; set; }

        public Group()
        {
        }

        public Group(string nameGroup, Guid accountId)
        {
            Name = nameGroup;
            CreaterId = accountId;
            CreatedDate = DateTime.UtcNow;
        }

        public Group(GroupDTO groupDTO, Guid accountId)
        {
            Name = groupDTO.Name;
            Id = groupDTO.GroupId;
            CreaterId = accountId;
        }

        public Group(Guid? id)
        {
            Id = id;
        }
        public IEnumerable<AccountStatusGroup> AccountStatusGroups { get; set; } = new List<AccountStatusGroup>();
        public IEnumerable<GroupTask>? GroupTasks { get; set; } = new List<GroupTask>();
    }
}