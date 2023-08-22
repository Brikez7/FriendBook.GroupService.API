namespace FriendBook.GroupService.API.Domain.Response
{
    public enum ServiceCode
    {
        EntityNotFound = 200,

        GroupCreated = 2001,
        GroupUpdated = 2002,
        GroupDeleted = 2003,
        GroupReadied = 2004,
        GroupAlreadyExists = 2005,
        GroupWithStatusMapped = 2006,

        AccountStatusGroupCreated = 2011,
        AccountStatusGroupUpdated = 2012,
        AccountStatusGroupDeleted = 2013,
        AccountStatusGroupReadied = 2014,
        AccountStatusGroupAlreadyExists = 2015,
        AccountStatusWithGroupMapped = 2016,

        GroupTaskCreated = 2021,
        GroupTaskUpdated = 2022,
        GroupTaskDeleted = 2023,
        GroupTaskReadied = 2024,
        GroupTaskAlreadyExists = 2025,
        GroupTaskNotDeleted = 2026,

        SubscribeTaskError = 2028,
        UnsubscribeTaskError = 2029,

        StageGroupTaskCreated = 2031,
        StageGroupTaskUpdated = 2032,
        StageGroupTaskDeleted = 2033,
        StageGroupTaskReadied = 2034,
        StageGroupTaskExists = 2035,

        UserNotExists = 2046,
        UserExists = 2047,
        UserNotAccess = 2048,

        GrpcProfileReadied = 2051,
        GrpcUsersReadied = 2052,

        HangfireUpdated = 2061,
        HangfireNotUpdated = 2062,
        HangfireUpdatedError = 2063,
        HangfireUpdatedZero = 2064,

        EntityIsValidated = 501,
        EntityIsNotValidated = 502,
    }
}