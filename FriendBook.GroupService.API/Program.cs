using FriendBook.GroupService.API.DAL;
using FriendBook.GroupService.API.Domain.Settings;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.GroupService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton(builder.Configuration);

            builder.AddRepositores();
            builder.AddAuthProperty();
            builder.AddValidators();
            builder.AddServices();
            builder.AddGrpcProperty();

            builder.AddODataProperty();
            builder.AddHangfire();
            builder.AddHostedServices();

            builder.Services.AddDbContext<GroupAppDBContext>(opt => opt.UseNpgsql(
                builder.Configuration.GetConnectionString(GroupAppDBContext.NameConnection)));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.AddCorsUI();

            app.UseAuthorization();
            app.MapControllers();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions());
            app.Run();
        }
    }
}