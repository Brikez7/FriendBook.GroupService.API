using FriendBook.GroupService.API.Domain.Entities;

namespace FriendBook.GroupService.API.Domain.DTO
{
    public class AccountGroupDTO
    {
        public AccountGroupDTO()
        {
        }


        public AccountGroupDTO(string groupName, Guid idGroupGuid,bool isAdmin)
        {
            GroupId = idGroupGuid;
            Name = groupName;
            IsAdmin = isAdmin;
        }
        public bool IsAdmin { get; set; } 
        public Guid GroupId { get; set; }
        public string Name { get; set; } = null!;
    }
}