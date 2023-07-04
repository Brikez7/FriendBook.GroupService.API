using FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;

namespace FriendBook.GroupService.API.Domain.Entities.Postgres
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

        public GroupTask(ResponseGroupTaskView groupDTO, Guid userId)
        {
            GroupId = groupDTO.GroupId;
            Name = groupDTO.Name;
            Description = groupDTO.Description;
            Status = groupDTO.Status;
            DateEndWork = groupDTO.DateEndWork;
            DateStartWork = groupDTO.DateStartWork;
            CreaterId = userId;
        }

        public GroupTask(RequestGroupTaskNew groupDTO, Guid userId)
        {
            GroupId = groupDTO.GroupId;
            Name = groupDTO.Name;
            Description = groupDTO.Description;
            DateEndWork = groupDTO.DateEndWork;
            CreaterId = userId;
            Team = new Guid[] { userId };
        }

        public GroupTask(RequestGroupTaskKey groupDTO)
        {
            GroupId = groupDTO.GroupId;
            Name = groupDTO.Name;
        }

        public GroupTask(RequestGroupTaskChanged groupTaskDTO)
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
    public enum StatusTask
    {
        Process = 0,
        Succsess = 1,
        Denied = 2,
        MissedDate = 3,
    }
}