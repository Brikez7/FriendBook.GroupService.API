namespace FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs
{
    public class ResponseTasksPage
    {
        public ResponseGroupTaskView[] TasksDTO { get; set; } = null!;
        public bool IsAdmin { get; set; }

        public ResponseTasksPage(ResponseGroupTaskView[] tasksDTO, bool isAdmin)
        {
            TasksDTO = tasksDTO;
            IsAdmin = isAdmin;
        }

        public ResponseTasksPage()
        {
        }
    }
}
