using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using NodaTime;
using NodaTime.Extensions;

namespace FriendBook.GroupService.API.Domain.Entities.Postgres
{
    public class GroupTask
    {
        public Guid? Id { get; set; }
        public Guid CreatorId { get; set; }
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid[] Team { get; set; } = Array.Empty<Guid>();
        public StatusTask Status { get; set; }
        public OffsetDateTime DateStartWork { get; set; } = DateTimeOffset.UtcNow.ToOffsetDateTime();
        public OffsetDateTime DateEndWork { get; set; }
        public GroupTask(){}

        public GroupTask(Guid id){ Id = id; }

        public GroupTask(ResponseGroupTaskView groupDTO, Guid userId)
        {
            GroupId = groupDTO.GroupId;
            Name = groupDTO.Name;
            Description = groupDTO.Description;
            Status = groupDTO.Status;
            DateEndWork = groupDTO.DateEndWork;
            DateStartWork = groupDTO.DateStartWork;
            CreatorId = userId;
        }

        public GroupTask(RequestNewGroupTask groupDTO, Guid userId)
        {
            GroupId = groupDTO.GroupId;
            Name = groupDTO.Name;
            Description = groupDTO.Description;
            DateEndWork = groupDTO.DateEndWork;
            CreatorId = userId;
            Team = new Guid[] { userId };
        }

        public GroupTask(UpdateGroupTaskDTO groupTaskDTO)
        {
            Id = groupTaskDTO.Id;
            Name = groupTaskDTO.Name;
            Description = groupTaskDTO.Description;
            Status = groupTaskDTO.Status;
            DateEndWork = groupTaskDTO.DateEndWork;
        }

        public Group? Group { get; set; }
    }
    public enum StatusTask
    {
        Process = 0,
        MissedDate  = 1,
        Denied = 2,
        Successes = 3,
    }
}