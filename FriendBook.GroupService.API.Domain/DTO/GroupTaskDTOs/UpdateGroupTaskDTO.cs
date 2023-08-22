using FriendBook.GroupService.API.Domain.Entities.Postgres;
using NodaTime;

namespace FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs
{
    public class UpdateGroupTaskDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public OffsetDateTime DateEndWork { get; set; }
        public StatusTask Status { get; set; }

        public UpdateGroupTaskDTO(Guid id, string newName, string description, OffsetDateTime dateEndWork, StatusTask status)
        {
            Id = id;
            Name = newName;
            Description = description;
            DateEndWork = dateEndWork;
            Status = status;
        }

        public UpdateGroupTaskDTO(){}
    }
}
