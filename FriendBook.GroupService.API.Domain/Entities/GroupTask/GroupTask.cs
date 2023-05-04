namespace FriendBook.GroupService.API.Domain.Entities
{
    public class GroupTask
    {
        public GroupTask(Guid id)
        {
            Id = id;
        }

        public Guid? Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Правки
        /// </summary>
        /// Можно выенсти в новую таблицу
        public Guid[] AccountsId { get; set; }
        public StatusTask Status { get; set; }
        public DateTime DateStartWork { get; set; }
        public DateTime DateEndWork { get; set; }

        public Group? Group { get; set; }
    }
}