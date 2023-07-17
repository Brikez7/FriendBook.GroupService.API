namespace FriendBook.GroupService.API.Domain.Response
{
    public enum Code
    {
        EntityNotFound = 200,

        GroupCreate = 2001,
        GroupUpdate = 2002,
        GroupDelete = 2003,
        GroupRead = 2004,
        GroupAlreadyExists = 2005,

        AccountStatusGroupCreate = 2011,
        AccountStatusGroupUpdate = 2012,
        AccountStatusGroupDelete = 2013,
        AccountStatusGroupRead = 2014,
        AccountStatusGroupAlreadyExists = 2015,

        GroupTaskCreate = 2021,
        GroupTaskUpdate = 2022,
        GroupTaskDelete = 2023,
        GroupTaskRead = 2024,
        GroupTaskAlreadyExists = 2025,

        SubscribeTaskError = 2028,
        UnsubscribeTaskError = 2029,

        StageGroupTaskCreate = 2031,
        StageGroupTaskUpdate = 2032,
        StageGroupTaskDelete = 2033,
        StageGroupTaskRead = 2034,
        StageGroupTaskExists = 2035,

        UserNotExists = 2046,
        UserExists = 2047,
        UserNotAccess = 2048,

        GrpcProfileRead = 2051,
        GrpcUsersRead = 2052,

        HangfireUpdated = 2061,
        HangfireNotUpdated = 2062,
        HangfireUpdatedError = 2063,
        HangfireUpdatedZero = 2064,

        EntityIsValidated = 501,
        EntityIsNotValidated = 502,
    }
}