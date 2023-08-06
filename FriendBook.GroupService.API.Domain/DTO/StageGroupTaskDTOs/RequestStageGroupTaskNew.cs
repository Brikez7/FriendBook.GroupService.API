namespace FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs
{
    public class RequestStageGroupTasNew
    {
        public string Name { get; set; }
        public Guid IdGroupTask { get; set; }

        public RequestStageGroupTasNew(string name, Guid groupId)
        {
            Name = name;
            IdGroupTask = groupId;
        }
    }
}
