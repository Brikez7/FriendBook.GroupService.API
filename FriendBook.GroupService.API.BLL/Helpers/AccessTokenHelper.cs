using FriendBook.GroupService.API.Domain.CustomClaims;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using System.Security.Claims;

namespace FriendBook.GroupService.API.BLL.Helpers
{
    public static class AccessTokenHelper
    {
        public static Lazy<DataAccessToken> CreateUser(IEnumerable<Claim> claims)
        {
            return new Lazy<DataAccessToken>(() => CreateUserToken(claims));
        }
        public static DataAccessToken CreateUserToken(IEnumerable<Claim> claims)
        {
            var login = claims.First(c => c.Type == CustomClaimType.Login).Value;
            var id = Guid.Parse(claims.First(c => c.Type == CustomClaimType.AccountId).Value);

            return new DataAccessToken(login, id);
        }
    }
}
