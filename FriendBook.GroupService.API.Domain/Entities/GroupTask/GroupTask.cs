using FriendBook.GroupService.API.Domain.DTO;

namespace FriendBook.GroupService.API.Domain.Entities
{
    public class GroupTask
    {
        public GroupTask()
        {
        }

        public GroupTask(Guid id)
        {
            Id = id;
        }

        public GroupTask(GroupTaskDTO groupDTO, Guid userId)
        {
            GroupId = groupDTO.GroupId;
            Name = groupDTO.Name;
            Description = groupDTO.Description;
            Status = groupDTO.StatusTask;
            DateEndWork = groupDTO.TaskEnd;
            DateStartWork = groupDTO.TaskStart;
        }

        public Guid? Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid[] AccountsId { get; set; }
        public StatusTask Status { get; set; }
        public DateTime DateStartWork { get; set; }
        public DateTime DateEndWork { get; set; }
        public Group? Group { get; set; }
    }
}