﻿namespace FriendBook.GroupService.API.Domain.DTO
{
    public class ProfileDTO
    {
        public ProfileDTO(Guid? id, string login, string? fullName)
        {
            Login = login;
            FullName = fullName;
            Id = (Guid)id;
        }

        public string Login { get; set; } = null!;
        public string? FullName { get; set; }
        public Guid Id { get; set; }
    }
}
