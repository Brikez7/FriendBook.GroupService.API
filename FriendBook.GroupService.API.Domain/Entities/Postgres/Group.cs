using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
using NodaTime;
using NodaTime.Extensions;

namespace FriendBook.GroupService.API.Domain.Entities.Postgres
{
    public class Group
    {
        public Guid? Id { get; set; }
        public Guid CreatorId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedDate { get; set; }

        public Group()
        {
        }

        public Group(string nameGroup, Guid accountId)
        {
            Name = nameGroup;
            CreatorId = accountId;
            CreatedDate = DateTime.Now;
        }

        public Group(ResponseGroupView groupDTO, Guid accountId)
        {
            Name = groupDTO.Name;
            Id = groupDTO.GroupId;
            CreatorId = accountId;
        }

        public Group(Guid? id)
        {
            Id = id;
        }
        public IEnumerable<AccountStatusGroup> AccountStatusGroups { get; set; } = new List<AccountStatusGroup>();
        public IEnumerable<GroupTask>? GroupTasks { get; set; } = new List<GroupTask>();
    }
}