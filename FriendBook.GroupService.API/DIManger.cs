using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.Entities;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.OData;
using FriendBook.GroupService.API.Middleware;
using FriendBook.GroupService.API.BackgroundHostedService;
using FriendBook.GroupService.API.DAL.Repositories.Repositories;

namespace FriendBook.GroupService.API
{
    public static class DIManger
    {
        public static void AddRepositores(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddScoped<IGroupRepository, GroupRepository>();
            webApplicationBuilder.Services.AddScoped<IAccountStatusGroupRepository, AccountStatusGroupRepository>();
        }

        public static void AddServices(this WebApplicationBuilder webApplicationBuilder)
        {
        }


        public static void AddODataProperty(this WebApplicationBuilder webApplicationBuilder)
        {
            var odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.EntitySet<Group>("Group");

            webApplicationBuilder.Services.AddControllers().AddOData(opt =>
            {
                opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(5000);
                opt.TimeZone = TimeZoneInfo.Utc;
            });
        }

        public static void AddAuthProperty(this WebApplicationBuilder webApplicationBuilder)
        {
            var secretKey = webApplicationBuilder.Configuration.GetSection("JWTSettings:SecretKey").Value;
            var issuer = webApplicationBuilder.Configuration.GetSection("JWTSettings:Issuer").Value;
            var audience = webApplicationBuilder.Configuration.GetSection("JWTSettings:Audience").Value;
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            webApplicationBuilder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuerSigningKey = true,
                };
            });
        }

        public static void AddHostedServices(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddHostedService<CheckDBHostedService>();
        }

        public static void AddMiddleware(this WebApplication webApplication)
        {
            webApplication.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}