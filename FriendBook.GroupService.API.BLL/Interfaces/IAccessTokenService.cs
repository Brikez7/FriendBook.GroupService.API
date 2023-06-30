using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.API.Domain.UserToken;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.BLL.Interfaces
{
    public interface IAccessTokenService
    {
        public Lazy<TokenAuth> CreateUser(IEnumerable<Claim> claims);
        public BaseResponse<TokenAuth?> CreateUserTokenTryEmpty(IEnumerable<Claim> claims);
    }
}
