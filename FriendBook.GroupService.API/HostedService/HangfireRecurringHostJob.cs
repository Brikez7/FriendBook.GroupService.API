using FriendBook.GroupService.API.BLL.Interfaces;
using Hangfire;

namespace FriendBook.GroupService.API.HostedService
{
    public class HangfireRecurringHostJob : IHostedService
    {
        private IGroupTaskService? _groupTaskService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public HangfireRecurringHostJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            _groupTaskService = scope.ServiceProvider.GetRequiredService<IGroupTaskService>();

            RecurringJob.AddOrUpdate("UpdateStatusTask", () => _groupTaskService!.UpdateStatusInGroupTasks(), Cron.Daily);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
             return Task.CompletedTask;
        }
    }
}
