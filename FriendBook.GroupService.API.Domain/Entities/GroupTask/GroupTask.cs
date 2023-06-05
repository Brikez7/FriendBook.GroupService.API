using FriendBook.GroupService.API.Domain.DTO.GroupTasksDTO;

namespace FriendBook.GroupService.API.Domain.Entities
{
    public class GroupTask
    {
        public Guid? Id { get; set; }
        public Guid CreaterId { get; set; }
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid[] Team { get; set; } = new Guid[0];
        public StatusTask Status { get; set; }
        public DateTime DateStartWork { get; set; } = DateTime.Now;
        public DateTime DateEndWork { get; set; }
        public GroupTask()
        {
        }

        public GroupTask(Guid id)
        {
            Id = id;
        }

        public GroupTask(GroupTaskViewDTO groupDTO, Guid userId)
        {
            GroupId = groupDTO.GroupId;
            Name = groupDTO.Name;
            Description = groupDTO.Description;
            Status = groupDTO.Status;
            DateEndWork = groupDTO.DateEndWork;
            DateStartWork = groupDTO.DateStartWork;
            CreaterId = userId;
        }

        public GroupTask(GroupTaskNewDTO groupDTO, Guid userId)
        {
            GroupId = groupDTO.GroupId;
            Name = groupDTO.Name;
            Description = groupDTO.Description;
            DateEndWork = groupDTO.DateEndWork;
            CreaterId = userId;
            Team = new Guid[] { userId };
        }

        public GroupTask(GroupTaskKeyDTO groupDTO)
        {
            GroupId = groupDTO.GroupId;
            Name = groupDTO.Name;
        }

        public GroupTask(GroupTaskChangedDTO groupTaskDTO)
        {
            GroupId = groupTaskDTO.GroupId;
            Name = groupTaskDTO.OldName;
            Description = groupTaskDTO.Description;
            Status = groupTaskDTO.Status;
            DateEndWork = groupTaskDTO.DateEndWork;
        }


        public Group? Group { get; set; }
/*        public IEnumerable<AccountStatusGroup>? AccountsStatusGroup { get; set; } = new List<AccountStatusGroup>();*/
    }
}