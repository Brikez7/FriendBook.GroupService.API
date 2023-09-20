using FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using NodaTime;
using NodaTime.Extensions;

namespace FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs
{
    public class ResponseGroupTaskView
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public StatusTask Status { get; set; } = StatusTask.Process;
        public OffsetDateTime DateEndWork { get; set; }
        public OffsetDateTime DateStartWork { get; set; } = DateTimeOffset.UtcNow.ToOffsetDateTime();
        public string[]? Users { get; set; }
        public ResponseStageGroupTaskIcon[] StagesGroupTask { get; set; } = Array.Empty<ResponseStageGroupTaskIcon>();

        public ResponseGroupTaskView(GroupTask groupTask, string[] users, ResponseStageGroupTaskIcon[] stagesGroupTask)
        {
            Id = (Guid)groupTask.Id!;
            GroupId = groupTask.GroupId;
            Name = groupTask.Name;
            Description = groupTask.Description;
            Status = groupTask.Status;
            DateEndWork = groupTask.DateEndWork;
            DateStartWork = groupTask.DateStartWork;
            Users = users;
            StagesGroupTask = stagesGroupTask;
        }

        public ResponseGroupTaskView(GroupTask groupTask, ResponseStageGroupTaskIcon[] stagesGroupTask)
        {
            Id = (Guid)groupTask.Id!;
            GroupId = groupTask.GroupId;
            Name = groupTask.Name;
            Description = groupTask.Description;
            Status = groupTask.Status;
            DateEndWork = groupTask.DateEndWork;
            DateStartWork = groupTask.DateStartWork;
            StagesGroupTask = stagesGroupTask;
        }

        public ResponseGroupTaskView(){}
    }
}
