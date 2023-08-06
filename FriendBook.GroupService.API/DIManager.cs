using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using System.Text;
using FriendBook.GroupService.API.Middleware;
using FriendBook.GroupService.API.BackgroundHostedService;
using FriendBook.GroupService.API.DAL.Repositories;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.BLL.Services;
using FriendBook.GroupService.API.Domain.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Hangfire;
using Hangfire.PostgreSql;
using FriendBook.GroupService.API.DAL;
using Microsoft.EntityFrameworkCore;
using FriendBook.GroupService.API.HostedService;
using MongoDB.Driver;
using FriendBook.GroupService.API.Domain.Entities.MongoDB;
using Microsoft.Extensions.Options;
using FriendBook.GroupService.API.BLL.GrpcServices;
using Hangfire.Client;
using Hangfire.States;
using Npgsql;

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
        public static void AddGrpc(this WebApplicationBuilder webApplicationBuilder) 
        {
            webApplicationBuilder.Services.Configure<GrpcSettings>(webApplicationBuilder.Configuration.GetSection(GrpcSettings.Name));
        }
        public static void AddValidators(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddValidators();
        }
        public static void AddServices(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddScoped<IGroupService, BLL.Services.GroupService>();
            webApplicationBuilder.Services.AddScoped<IAccountStatusGroupService, AccountStatusGroupService>();
            webApplicationBuilder.Services.AddScoped<IGroupTaskService, GroupTaskService>();

            webApplicationBuilder.Services.AddScoped<IGrpcClient, GrpcClient>();
        }
        public static void AddHangfire(this WebApplicationBuilder webApplicationBuilder) 
        {
            webApplicationBuilder.Services.AddSingleton<JobStorage>(x =>
            {
                var jobStorage = new PostgreSqlStorage(webApplicationBuilder.Configuration.GetConnectionString("HangfireNpgConnectionString"));
                return jobStorage;
            });
            webApplicationBuilder.Services.AddSingleton<IBackgroundJobFactory>(x => 
            {
                var backgroundJobFactory = new BackgroundJobFactory();
                return backgroundJobFactory;
            });
            webApplicationBuilder.Services.AddSingleton<IBackgroundJobStateChanger>(x => 
            {
                var backgroundJobStateChanger = new BackgroundJobStateChanger();
                return backgroundJobStateChanger;
            });

            webApplicationBuilder.Services.AddSingleton<IBackgroundJobClient>(x => new BackgroundJobClient(
                x.GetRequiredService<JobStorage>(),
                x.GetRequiredService<IBackgroundJobFactory>(),
                x.GetRequiredService<IBackgroundJobStateChanger>()));

            webApplicationBuilder.Services.AddSingleton<IRecurringJobManager>(x => new RecurringJobManager(
                x.GetRequiredService<JobStorage>(),
                x.GetRequiredService<IBackgroundJobFactory>()));

            webApplicationBuilder.Services.AddHangfire(configuration =>
            {
                configuration.UseSimpleAssemblyNameTypeSerializer()
                             .UseRecommendedSerializerSettings()
                             .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                             .UsePostgreSqlStorage(webApplicationBuilder.Configuration.GetConnectionString("HangfireNpgConnectionString"));
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
            webApplicationBuilder.Services.AddDbContext<GroupDBContext>(opt => opt.UseNpgsql(
                webApplicationBuilder.Configuration.GetConnectionString(GroupDBContext.NameConnection), o => o.UseNodaTime()));
        }
        public static void AddAuth(this WebApplicationBuilder webApplicationBuilder)
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