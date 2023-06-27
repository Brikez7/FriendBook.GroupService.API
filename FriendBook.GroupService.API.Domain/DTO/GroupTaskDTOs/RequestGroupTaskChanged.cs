using FriendBook.GroupService.API.Domain.Entities;

namespace FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs
{
    public class RequestGroupTaskChanged
    {
        public Guid GroupId { get; set; }
        public string OldName { get; set; } = null!;
        public string NewName { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public DateTime DateEndWork { get; set; }
        public StatusTask Status { get; set; }
        public RequestGroupTaskChanged(Guid groupId, string oldName, string newName, string description, DateTime dateEndWork, StatusTask statusTask)
        {
            GroupId = groupId;
            OldName = oldName;
            NewName = newName;
            Description = description;
            DateEndWork = dateEndWork;
            Status = statusTask;
        }

        public RequestGroupTaskChanged()
        {
        }
    }
}
