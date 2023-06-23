namespace FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs
{
    public class ResponseProfile
    {
        public ResponseProfile(Guid id, string login, string? fullName)
        {
            Login = login;
            FullName = fullName;
            Id = id;
        }

        public string Login { get; set; } = null!;
        public string? FullName { get; set; }
        public Guid Id { get; set; }
    }
}
