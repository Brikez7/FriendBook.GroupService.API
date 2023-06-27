using FriendBook.GroupService.API.Domain.Entities;

namespace FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs
{
    public class ResponseGroupTaskView
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public StatusTask Status { get; set; } = StatusTask.Process;
        public DateTime DateEndWork { get; set; }
        public DateTime DateStartWork { get; set; } = DateTime.UtcNow;
        public string[]? Users { get; set; }
        public ResponseGroupTaskView(GroupTask groupTask, string user)
        {
            GroupId = groupTask.GroupId;
            Name = groupTask.Name;
            Description = groupTask.Description;
            Status = groupTask.Status;
            DateEndWork = groupTask.DateEndWork;
            DateStartWork = groupTask.DateStartWork;
            Users = new string[] { user };
        }

        public ResponseGroupTaskView(GroupTask groupTask, string[] users)
        {
            GroupId = groupTask.GroupId;
            Name = groupTask.Name;
            Description = groupTask.Description;
            Status = groupTask.Status;
            DateEndWork = groupTask.DateEndWork;
            DateStartWork = groupTask.DateStartWork;
            Users = users;
        }

        public ResponseGroupTaskView(GroupTask groupTask)
        {
            GroupId = groupTask.GroupId;
            Name = groupTask.Name;
            Description = groupTask.Description;
            Status = groupTask.Status;
            DateEndWork = groupTask.DateEndWork;
            DateStartWork = groupTask.DateStartWork;
        }

        public ResponseGroupTaskView()
        {
        }
    }
}
