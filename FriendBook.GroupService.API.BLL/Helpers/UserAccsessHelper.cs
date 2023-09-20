using FriendBook.GroupService.API.Domain.Entities.Postgres;

namespace FriendBook.GroupService.API.BLL.Helpers
{
    public static class UserAccessHelper
    {
        public static IQueryable<AccountStatusGroup> CheckAdmin(this IQueryable<AccountStatusGroup> accountStatusGroups,Guid accountStatusGroupId)  
            => accountStatusGroups.Where(x => x.RoleAccount > RoleAccount.Default && x.Id == accountStatusGroupId);

        public static IQueryable<AccountStatusGroup> CheckCreator(this IQueryable<AccountStatusGroup> accountStatusGroups, Guid accountStatusGroupId)
            => accountStatusGroups.Where(x => x.RoleAccount == RoleAccount.Creator && x.Id == accountStatusGroupId);

        public static IQueryable<AccountStatusGroup> CheckUserExists(this IQueryable<AccountStatusGroup> accountStatusGroups, Guid accountStatusGroupId)
            => accountStatusGroups.Where(x => x.Id == accountStatusGroupId);
    }
}
