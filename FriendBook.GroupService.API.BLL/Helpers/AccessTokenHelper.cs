using FriendBook.GroupService.API.Domain.CustomClaims;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using System.Security.Claims;

namespace FriendBook.GroupService.API.BLL.Helpers
{
    public static class AccessTokenHelper
    {
        public static Lazy<AccessToken> CreateUser(IEnumerable<Claim> claims)
        {
            return new Lazy<AccessToken>(() => CreateUserToken(claims));
        }
        public static AccessToken CreateUserToken(IEnumerable<Claim> claims)
        {
            var login = claims.First(c => c.Type == CustomClaimType.Login).Value;
            var id = Guid.Parse(claims.First(c => c.Type == CustomClaimType.AccountId).Value);

            return new AccessToken(login, id);
        }
    }
}
