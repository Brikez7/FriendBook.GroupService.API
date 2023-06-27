namespace FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs
{
    public class ResponseAccountGroup
    {
        public ResponseAccountGroup()
        {
        }


        public ResponseAccountGroup(string groupName, Guid idGroupGuid, bool isAdmin)
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