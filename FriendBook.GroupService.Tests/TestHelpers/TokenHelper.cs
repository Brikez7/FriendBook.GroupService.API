using FriendBook.GroupService.API.Domain.CustomClaims;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Settings;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FriendBook.GroupService.Tests.TestHelpers
{
    internal static class TokenHelper
    {
        public static string GenerateAccessToken(DataAccessToken account, JWTSettings jWTSettings)
        {
            List<Claim> claims = new()
            {
                new Claim(CustomClaimType.Login,account.Login),
                new Claim(CustomClaimType.AccountId, account.Id.ToString()!)
            };
            var jwtToken = GenerateToken(jWTSettings.AccessTokenSecretKey, jWTSettings.Issuer, jWTSettings.Audience, jWTSettings.AccessTokenExpirationMinutes, claims);
            return jwtToken;
        }
        private static string GenerateToken(string secretKey, string issuer, string audience, double expires, IEnumerable<Claim>? claims = null)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var jwtToken = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
                    expires: DateTime.Now.Add(TimeSpan.FromMinutes(expires)),
                    notBefore: DateTime.Now,
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
