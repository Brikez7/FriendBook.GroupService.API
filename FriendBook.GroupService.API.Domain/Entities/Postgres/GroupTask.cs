using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using NodaTime;
using NodaTime.Extensions;

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
        public OffsetDateTime DateStartWork { get; set; } = DateTimeOffset.UtcNow.ToOffsetDateTime();
        public OffsetDateTime DateEndWork { get; set; }
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

        public GroupTask(RequestNewGroupTask groupDTO, Guid userId)
        {
            GroupId = groupDTO.GroupId;
            Name = groupDTO.Name;
            Description = groupDTO.Description;
            DateEndWork = groupDTO.DateEndWork;
            CreaterId = userId;
            Team = new Guid[] { userId };
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
    }
    public enum StatusTask
    {
        Process = 0,
        Successes = 1,
        Denied = 2,
        MissedDate = 3,
    }
}