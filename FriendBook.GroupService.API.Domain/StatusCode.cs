namespace FriendBook.GroupService.API.Domain
{
    public enum StatusCode
    {
        EntityNotFound = 0,

        GroupCreate = 1,
        GroupUpdate = 2,
        GroupDelete = 3,
        GroupRead = 4,



        OK = 200,
        OKNoContent = 204,
        IdNotFound = 401,
        InternalServerError = 500,
    }
}