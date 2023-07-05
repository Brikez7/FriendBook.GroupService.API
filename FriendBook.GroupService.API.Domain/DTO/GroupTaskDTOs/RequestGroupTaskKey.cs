namespace FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs
{
    public class RequestGroupTaskKey
    {
        public Guid GroupId { get; set; }
        public Guid GroupTaskId { get; set; }

        public RequestGroupTaskKey()
        {
        }

        public RequestGroupTaskKey(Guid groupId, Guid groupTaskId)
        {
            GroupId = groupId;
            GroupTaskId = groupTaskId;
        }
    }
}
