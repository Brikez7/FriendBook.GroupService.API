using FriendBook.GroupService.API.Domain.DTO.GroupTasksDTO;

namespace FriendBook.GroupService.API.Domain.DTO
{
    public class TasksPageDTO
    {
        public GroupTaskViewDTO[] TasksDTO { get; set; } = null!;
        public bool IsAdmin { get; set; }

        public TasksPageDTO(GroupTaskViewDTO[] tasksDTO, bool isAdmin)
        {
            TasksDTO = tasksDTO;
            IsAdmin = isAdmin;
        }
    }
}
