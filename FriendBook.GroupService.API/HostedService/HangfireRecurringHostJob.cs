using FriendBook.GroupService.API.BLL.Interfaces;
using Hangfire;

namespace FriendBook.GroupService.API.HostedService
{
    public class HangfireRecurringHostJob : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public HangfireRecurringHostJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var groupTaskService = scope.ServiceProvider.GetRequiredService<IGroupTaskService>();
            var backgroundJobClient = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

            backgroundJobClient.AddOrUpdate("UpdateStatusTask", () => groupTaskService!.UpdateStatusInGroupTasks(), Cron.Daily);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
             return Task.CompletedTask;
        }
    }
}
