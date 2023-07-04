namespace FriendBook.GroupService.API.Domain.Response
{
    public enum StatusCode
    {
        EntityNotFound = 0,

        UserNotExists = 1,
        UserExists = 2,
        UserNotAccess = 3,

        GroupCreate = 11,
        GroupUpdate = 12,
        GroupDelete = 13,
        GroupRead = 14,
        GroupExists = 15,

        GrpcProphileRead = 17,
        GrpcUsersRead= 18,

        AccountStatusGroupCreate = 21,
        AccountStatusGroupUpdate = 22,
        AccountStatusGroupDelete = 23,
        AccountStatusGroupRead = 24,
        AccountStatusGroupExists = 25,

        ProphileMapped = 28,

        GroupTaskCreate = 31,
        GroupTaskUpdate = 32,
        GroupTaskDelete = 33,
        GroupTaskRead = 34,
        GroupTaskExists = 35,
        GroupTaskNotUpdated = 36,

        SubscribeErrror = 41,
        UnsubscribeError = 42,

        TokenNotValid = 48,

        EntityIsValid = 54,
        ErrorValidation = 55,

        HangfireUpdated = 61, 
        HangfireUpdatedError = 62,

        StageGroupTaskCreate = 71,
        StageGroupTaskUpdate = 72,
        StageGroupTaskDelete = 73,
        StageGroupTaskRead = 74,
        StageGroupTaskExists = 75,
        StageGroupTaskNotUpdated = 76,

        OK = 200,
        OKNoContent = 204,
        IdNotFound = 401,
        InternalServerError = 500,
    }
}