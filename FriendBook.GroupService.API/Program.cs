using Hangfire;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace FriendBook.GroupService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton(builder.Configuration);

            builder.AddMongoDB();
            builder.AddPostgresDB();

            builder.AddRepositores();
            builder.AddValidators();
            builder.AddServices();

            builder.AddGrpc();
            builder.AddAuth();
            builder.AddHangfire();
            builder.AddHostedServices();

            builder.Services.AddControllers().AddNewtonsoftJson(s => s.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));
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