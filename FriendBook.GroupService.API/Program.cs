using FriendBook.GroupService.API.DAL;
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
            builder.AddServices();
            builder.AddAuthProperty();
            builder.AddODataProperty();
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

            app.UseCors(builder => builder
               .WithOrigins("http://localhost:3000")
               .AllowAnyHeader()
               .AllowAnyMethod());

            app.UseCors();
            app.UseRouting();
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}