using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.Entities;
using Microsoft.AspNet.OData.Builder;
using System.Text;
using Microsoft.AspNetCore.OData;
using FriendBook.GroupService.API.Middleware;
using FriendBook.GroupService.API.BackgroundHostedService;
using FriendBook.GroupService.API.DAL.Repositories;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.BLL.Services;
using FluentValidation;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
using FriendBook.GroupService.API.Domain.Validators.GroupTaskDTOValidators;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Validators.AccountStatusGroupDTOValidators;
using FriendBook.GroupService.API.Domain.Validators.GroupDTOValidators;
using FriendBook.GroupService.API.Domain.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FriendBook.IdentityServer.API.BLL.Services;
using FriendBook.IdentityServer.API.BLL.Interfaces;
using Hangfire;
using Hangfire.PostgreSql;
using FriendBook.GroupService.API.DAL;
using Microsoft.EntityFrameworkCore;
using FriendBook.GroupService.API.HostedService;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using MongoDB.Driver;
using FriendBook.GroupService.API.Domain.Entities.MongoDB;
using Microsoft.Extensions.Options;

namespace FriendBook.GroupService.API
{
    public static class DIManager
    {
        public static void AddRepositores(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddScoped<IGroupRepository, GroupRepository>();
            webApplicationBuilder.Services.AddScoped<IAccountStatusGroupRepository, AccountStatusGroupRepository>();
            webApplicationBuilder.Services.AddScoped<IGroupTaskRepository, GroupTaskRepository>();
            webApplicationBuilder.Services.AddScoped<IStageGroupTaskRepository, StageGroupTaskRepository>();
        }
        public static void AddGrpcProperty(this WebApplicationBuilder webApplicationBuilder) 
        {
            webApplicationBuilder.Services.Configure<GrpcSettings>(webApplicationBuilder.Configuration.GetSection(GrpcSettings.Name));
        }
        public static void AddValidators(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddScoped<IValidator<AccountStatusGroupDTO>, ValidatorAccountStatusGroupDTO>();

            webApplicationBuilder.Services.AddScoped<IValidator<GroupDTO>, ValidatorGroupDTO>();

            webApplicationBuilder.Services.AddScoped<IValidator<RequestGroupTaskNew>, ValidatorRequestGroupTaskNew>();
            webApplicationBuilder.Services.AddScoped<IValidator<RequestGroupTaskChanged>, ValidatorRequestGroupTaskChanged>();
            webApplicationBuilder.Services.AddScoped<IValidator<RequestGroupTaskKey>, ValidatorRequestGroupTaskKey>();
        }
        public static void AddServices(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddScoped<IContactGroupService, ContactGroupService>();
            webApplicationBuilder.Services.AddScoped<IAccountStatusGroupService, AccountStatusGroupService>();
            webApplicationBuilder.Services.AddScoped<IGroupTaskService, GroupTaskService>();

            webApplicationBuilder.Services.AddScoped<IGrpcService, GrpcService>();
            webApplicationBuilder.Services.AddScoped<IAccessTokenService, AccessTokenService>();

            webApplicationBuilder.Services.AddScoped<IValidationService<AccountStatusGroupDTO>, ValidationService<AccountStatusGroupDTO>>();
            webApplicationBuilder.Services.AddScoped<IValidationService<GroupDTO>, ValidationService<GroupDTO>>();

            webApplicationBuilder.Services.AddScoped<IValidationService<RequestGroupTaskNew>, ValidationService<RequestGroupTaskNew>>();
            webApplicationBuilder.Services.AddScoped<IValidationService<RequestGroupTaskChanged>, ValidationService<RequestGroupTaskChanged>>();
            webApplicationBuilder.Services.AddScoped<IValidationService<RequestGroupTaskKey>, ValidationService<RequestGroupTaskKey>>();
        }
        public static void AddHangfire(this WebApplicationBuilder webApplicationBuilder) 
        {
            webApplicationBuilder.Services.AddHangfire(configuration =>
            {
                configuration.UseSimpleAssemblyNameTypeSerializer()
                             .UseRecommendedSerializerSettings()
                             .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                             .UsePostgreSqlStorage(webApplicationBuilder.Configuration.GetConnectionString(HangfireContext.NameConnection));
            });
            webApplicationBuilder.Services.AddHangfireServer();
        }
        public static void AddMongoDB(this WebApplicationBuilder webApplicationBuilder) 
        {
            webApplicationBuilder.Services.Configure<MongoDBSettings>(
                webApplicationBuilder.Configuration.GetSection(MongoDBSettings.Name));

            webApplicationBuilder.Services.AddSingleton<IMongoClient>(provider => 
                new MongoClient(provider.GetRequiredService<IOptions<MongoDBSettings>>().Value.MongoDBConnectionString)
            );

            webApplicationBuilder.Services.AddSingleton(provider =>
            {
                var mongoClient = provider.GetRequiredService<IMongoClient>();
                return mongoClient.GetDatabase(provider.GetRequiredService<IOptions<MongoDBSettings>>().Value.Database);
            });

            webApplicationBuilder.Services.AddScoped(provider =>
            {
                var database = provider.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<StageGroupTask>(provider.GetRequiredService<IOptions<MongoDBSettings>>().Value.Collection);
            });

        }
        public static void AddPostgresDB(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddDbContext<GroupAppDBContext>(opt => opt.UseNpgsql(
                 webApplicationBuilder.Configuration.GetConnectionString(GroupAppDBContext.NameConnection)));
        }
        public static void AddODataProperty(this WebApplicationBuilder webApplicationBuilder)
        {
            var odataBuilder = new ODataConventionModelBuilder();

            odataBuilder.EntitySet<Group>("Group");
            odataBuilder.EntitySet<AccountStatusGroup>("AccountStatusGroup");
            odataBuilder.EntitySet<GroupTask>("GroupTask");

            webApplicationBuilder.Services.AddControllers().AddOData(opt =>
            {
                opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(5000);
                opt.TimeZone = TimeZoneInfo.Utc;
            });
        }

        public static void AddAuthProperty(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddHttpContextAccessor();
            webApplicationBuilder.Services.Configure<JWTSettings>(webApplicationBuilder.Configuration.GetSection(JWTSettings.Name));

            var jwtSettings = webApplicationBuilder.Configuration.GetSection(JWTSettings.Name).Get<JWTSettings>() ??
                throw new InvalidOperationException($"{JWTSettings.Name} not found in sercret.json");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.AccessTokenSecretKey!));

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
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuerSigningKey = true
                };
            });
        }

        public static void AddHostedServices(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddHostedService<CheckDBHostedService>();
            webApplicationBuilder.Services.AddHostedService<HangfireRecurringHostJob>();
        }

        public static void AddMiddleware(this WebApplication webApplication)
        {
            webApplication.UseMiddleware<ExceptionHandlingMiddleware>();
        }
        public static void AddCorsUI(this WebApplication webApplication)
        {
            var urlApp = webApplication.Configuration.GetSection(AppUISetting.Name).Get<AppUISetting>() ??
                throw new InvalidOperationException($"{AppUISetting.Name} not found in appsettings.json");

            webApplication.UseCors(builder => builder
                          .WithOrigins(urlApp.AppURL)
                          .AllowAnyHeader()
                          .AllowAnyMethod());
        }
    }
}