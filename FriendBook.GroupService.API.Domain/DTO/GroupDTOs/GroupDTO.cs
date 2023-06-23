using FriendBook.GroupService.API.Domain.Entities;

namespace FriendBook.GroupService.API.Domain.DTO.GroupDTOs
{
    public class GroupDTO
    {
        public GroupDTO()
        {
        }

        public GroupDTO(Group createdGroup)
        {
            GroupId = (Guid)createdGroup.Id!;
            CreatedDate = createdGroup.CreatedDate;
            Name = createdGroup.Name;
        }

        public GroupDTO(string groupName, Guid idGroupGuid)
        {
            GroupId = idGroupGuid;
            Name = groupName;
        }

        public Guid GroupId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Name { get; set; } = null!;
    }
}