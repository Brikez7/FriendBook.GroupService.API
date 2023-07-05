using FriendBook.GroupService.API.BLL.Interfaces;
using Hangfire;

namespace FriendBook.GroupService.API.HostedService
{
    public class HangfireRecurringHostJob : BackgroundService
    {
        private IGroupTaskService? _groupTaskService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public HangfireRecurringHostJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task UpdateStatusInGroupTasksAsync()
        {
            await _groupTaskService!.UpdateStatusInGroupTasks();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            _groupTaskService = scope.ServiceProvider.GetRequiredService<IGroupTaskService>();

            RecurringJob.AddOrUpdate("UpdateStatusTask", () => UpdateStatusInGroupTasksAsync(), Cron.Daily);

            return;
        }
    }
}
