﻿using FriendBook.GroupService.API.Domain.DTO;

namespace FriendBook.GroupService.API.Domain.Entities
{
    public class Group
    {
        public Guid? Id { get; set; }
        public Guid AccountId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedDate { get; set; }

        public Group()
        {
        }

        public Group(GroupDTO commentDTO, Guid accountId)
        {
            Id = commentDTO.GuidGroupId; 
            Name = commentDTO.Name;
            AccountId = accountId;
            CreatedDate = DateTime.Now;
        }

        public Group(Guid? id)
        {
            Id = id;
        }

        public IEnumerable<AccountStatusGroup>? AccountStatusGroups { get; set; } 
    }
}