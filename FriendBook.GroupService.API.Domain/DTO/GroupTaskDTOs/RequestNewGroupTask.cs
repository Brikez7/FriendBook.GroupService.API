﻿using NodaTime;

namespace FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs
{
    public class RequestNewGroupTask
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public OffsetDateTime DateEndWork { get; set; }
        public OffsetDateTime CreateDate { get; set; }
        public RequestNewGroupTask(Guid groupId, string name, string description, OffsetDateTime dateEndWork, OffsetDateTime createDate)
        {
            GroupId = groupId;
            Name = name;
            Description = description;
            DateEndWork = dateEndWork;
            CreateDate = createDate;
        }

        public RequestNewGroupTask()
        {
        }
    }
}
