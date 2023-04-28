namespace FriendBook.GroupService.API.Domain
{
    public enum StatusCode
    {
        EntityNotFound = 0,

        GroupCreate = 11,
        GroupUpdate = 12,
        GroupDelete = 13,
        GroupRead = 14,

        AccountStatusGroupCreate = 21,
        AccountStatusGroupUpdate = 22,
        AccountStatusGroupDelete = 23,
        AccountStatusGroupRead = 24,

        OK = 200,
        OKNoContent = 204,
        IdNotFound = 401,
        InternalServerError = 500,
    }
}