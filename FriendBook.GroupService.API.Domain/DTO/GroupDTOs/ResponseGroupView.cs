using FriendBook.GroupService.API.Domain.Entities.Postgres;

namespace FriendBook.GroupService.API.Domain.DTO.GroupDTOs
{
    public class ResponseGroupView
    {
        public Guid GroupId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; } = null!;

        public ResponseGroupView(){}

        public ResponseGroupView(Group createdGroup)
        {
            GroupId = (Guid)createdGroup.Id!;
            CreatedDate = createdGroup.CreatedDate;
            Name = createdGroup.Name;
        }
    }
}