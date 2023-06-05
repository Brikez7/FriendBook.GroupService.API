using FriendBook.GroupService.API.Domain.Entities;

namespace FriendBook.GroupService.API.Domain.DTO.GroupTasksDTO
{
    public class GroupTaskViewDTO
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public StatusTask Status { get; set; } = StatusTask.Process;
        public DateTime DateEndWork { get; set; }
        public DateTime DateStartWork { get; set; } = DateTime.UtcNow;
        public string[]? Users { get; set; }
        public GroupTaskViewDTO(GroupTask groupTask, string user)
        {
            GroupId = groupTask.GroupId;
            Name = groupTask.Name;
            Description = groupTask.Description;
            Status = groupTask.Status;
            DateEndWork = groupTask.DateEndWork;
            DateStartWork = groupTask.DateStartWork;
            Users = new string[] { user };
        }

        public GroupTaskViewDTO(GroupTask groupTask, string[] users)
        {
            GroupId = groupTask.GroupId;
            Name = groupTask.Name;
            Description = groupTask.Description;
            Status = groupTask.Status;
            DateEndWork = groupTask.DateEndWork;
            DateStartWork = groupTask.DateStartWork;
            Users = users;
        }

        public GroupTaskViewDTO(GroupTask groupTask)
        {
            GroupId = groupTask.GroupId;
            Name = groupTask.Name;
            Description = groupTask.Description;
            Status = groupTask.Status;
            DateEndWork = groupTask.DateEndWork;
            DateStartWork = groupTask.DateStartWork;
        }

        public GroupTaskViewDTO()
        {
        }
    }
}
