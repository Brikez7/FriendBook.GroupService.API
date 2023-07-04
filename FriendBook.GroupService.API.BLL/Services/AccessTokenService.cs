using FriendBook.GroupService.API.Domain.CustomClaims;
using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.API.Domain.UserToken;
using FriendBook.IdentityServer.API.BLL.Interfaces;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.BLL.Services
{
    public class AccessTokenService : IAccessTokenService
    {
        public AccessTokenService()
        {
        }

        public Lazy<TokenAuth> CreateUser(IEnumerable<Claim> claims)
        {
            return new Lazy<TokenAuth>(() => CreateUserToken(claims));
        }
        private static TokenAuth CreateUserToken(IEnumerable<Claim> claims)
        {
            var login = claims.First(c => c.Type == CustomClaimType.Login).Value;
            var id = Guid.Parse(claims.First(c => c.Type == CustomClaimType.AccountId).Value);

            return new TokenAuth(login, id);
        }
        public BaseResponse<TokenAuth?> CreateUserTokenTryEmpty(IEnumerable<Claim> claims)
        {
            var login = claims.FirstOrDefault(c => c.Type == CustomClaimType.Login)?.Value;
            var stringId = claims.FirstOrDefault(c => c.Type == CustomClaimType.AccountId)?.Value;

            if (stringId == null || login == null)
                return new StandartResponse<TokenAuth> { Message = "Access token not validated", StatusCode = StatusCode.TokenNotValid }!;

            var id = stringId is not null ? Guid.Parse(stringId) : Guid.Empty;

            return new StandartResponse<TokenAuth?> { Data = new TokenAuth(login, id) };
        }
    }
}
