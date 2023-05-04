using FriendBook.GroupService.API.Domain.DTO;

namespace FriendBook.GroupService.API.Domain.Entities
{
    public class GroupTask
    {
        private GroupTaskDTO groupDTO;
        private Guid userId;

        public GroupTask(Guid id)
        {
            Id = id;
        }

        public GroupTask(GroupTaskDTO groupDTO, Guid userId)
        {
            this.groupDTO = groupDTO;
            this.userId = userId;
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