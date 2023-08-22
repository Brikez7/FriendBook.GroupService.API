using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.API.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using MongoDB.Driver;
using FriendBook.GroupService.API.Domain.Entities.MongoDB;
using FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs;
using NodaTime.Extensions;
using FriendBook.GroupService.API.BLL.gRPCClients.AccountClient;
using FriendBook.GroupService.API.BLL.GrpcServices;
using MongoDB.Driver.Linq;

namespace FriendBook.GroupService.API.BLL.Services
{
    public class GroupTaskService : IGroupTaskService
    {
        private readonly IGroupTaskRepository _groupTaskRepository;
        private readonly IAccountStatusGroupRepository _accountStatusGroupRepository;
        private readonly IStageGroupTaskRepository _stageGroupTaskRepository;
        private readonly IGrpcClient _grpcService;
        public GroupTaskService(IGroupTaskRepository groupTaskRepository, IAccountStatusGroupRepository accountStatusGroupRepository, IStageGroupTaskRepository stageGroupTask,
            IGrpcClient grpcService)
        {
            _groupTaskRepository = groupTaskRepository;
            _accountStatusGroupRepository = accountStatusGroupRepository;
            _stageGroupTaskRepository = stageGroupTask;
            _grpcService = grpcService;
        }

        public async Task<BaseResponse<ResponseGroupTaskView>> CreateGroupTask(RequestNewGroupTask requestNewGroupTask, Guid adminId, string loginAdmin)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == adminId && x.IdGroup == requestNewGroupTask.GroupId && x.RoleAccount > RoleAccount.Default))
                return new StandardResponse<ResponseGroupTaskView> { Message = "Account not found or you not access create new group task", ServiceCode = ServiceCode.UserNotAccess };

            if (await _groupTaskRepository.GetAll().AnyAsync(x => x.Name == requestNewGroupTask.Name && x.GroupId == requestNewGroupTask.GroupId))
                return new StandardResponse<ResponseGroupTaskView> { Message = "Task with name already exists", ServiceCode = ServiceCode.GroupTaskAlreadyExists };

            var newGroupTask = new GroupTask(requestNewGroupTask, adminId);
            var addedGroupTask = await _groupTaskRepository.AddAsync(newGroupTask);

            var newStageGroupTask = new StageGroupTask((Guid)addedGroupTask.Id!, $"Start task: {addedGroupTask.Name}", "", requestNewGroupTask.CreateDate, requestNewGroupTask.CreateDate.LocalDateTime.ToDateTimeUnspecified());
            var result = await _stageGroupTaskRepository.AddAsync(newStageGroupTask);

            await _groupTaskRepository.SaveAsync();

            var listStage = new ResponseStageGroupTaskIcon[] { new ResponseStageGroupTaskIcon(result.Id, result.Name) };
            var viewDTO = new ResponseGroupTaskView(addedGroupTask, listStage){ Users = new string[] { loginAdmin } };

            return new StandardResponse<ResponseGroupTaskView>()
            {
                Data = viewDTO,
                ServiceCode = ServiceCode.GroupTaskCreated
            };
        }

        public async Task<BaseResponse<bool>> SubscribeGroupTask(Guid groupTaskId, Guid userId)
        {
            var task = await _groupTaskRepository.GetAll()
                                                 .Where(x => x.Id == groupTaskId)
                                                 .SingleOrDefaultAsync();
            if (task is null)
                return new StandardResponse<bool> { Message = "task not exists", ServiceCode = ServiceCode.EntityNotFound };

            if (task.Team.Any(t => t == userId))
                return new StandardResponse<bool> { Message = "You already subscribe in group", ServiceCode = ServiceCode.SubscribeTaskError };

            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == task.GroupId && userId == x.AccountId)) 
                return new StandardResponse<bool>{ Message = "You not exists in group", ServiceCode = ServiceCode.UserNotExists };

            task.Team = task.Team.Append(userId).ToArray();
            var updatedTask = _groupTaskRepository.Update(task);
            await _groupTaskRepository.SaveAsync();

            return new StandardResponse<bool>()
            {
                Data = updatedTask != null,
                ServiceCode = ServiceCode.GroupTaskUpdated
            };
        }
        public async Task<BaseResponse<bool>> UnsubscribeGroupTask(Guid groupTaskId, Guid userId)
        {
            var task = await _groupTaskRepository.GetAll()
                                     .Where(x => x.Id == groupTaskId)
                                     .SingleOrDefaultAsync();

            if (task is null)
                return new StandardResponse<bool> { Message = "Task not exists", ServiceCode = ServiceCode.EntityNotFound };

            if (!task.Team.Any(t => t == userId))
                return new StandardResponse<bool>{ Message = "You already unsubscribe in group", ServiceCode = ServiceCode.UnsubscribeTaskError };

            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == task.GroupId && userId == x.AccountId))
                return new StandardResponse<bool>{ Message = "Group not exists or you not been in group", ServiceCode = ServiceCode.EntityNotFound };

            task.Team = task.Team.Where(x => x != userId).ToArray();

            var updatedGroup = _groupTaskRepository.Update(task);
            await _groupTaskRepository.SaveAsync();

            return new StandardResponse<bool>()
            {
                Data = updatedGroup != null,
                ServiceCode = ServiceCode.GroupTaskUpdated
            };
        }

        public async Task<BaseResponse<UpdateGroupTaskDTO>> UpdateGroupTask(UpdateGroupTaskDTO requestUpdateGroupTask, Guid adminId)
        {
            var task = await _groupTaskRepository.GetAll().FirstOrDefaultAsync(x => requestUpdateGroupTask.Id == x.Id);

            if (task is null)
                return new StandardResponse<UpdateGroupTaskDTO> { Message = "Task not found", ServiceCode = ServiceCode.EntityNotFound };

            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == task.GroupId && x.AccountId == adminId && x.RoleAccount > RoleAccount.Default)) 
                return new StandardResponse<UpdateGroupTaskDTO> {Message = "You not exists in this group or you not have access update group task", ServiceCode = ServiceCode.UserNotAccess };
            
            task.Status = requestUpdateGroupTask.Status;
            task.DateEndWork = requestUpdateGroupTask.DateEndWork;
            task.Description = requestUpdateGroupTask.Description;
            task.Name = requestUpdateGroupTask.Name;

            var updatedGroupTask = _groupTaskRepository.Update(task);
            await _groupTaskRepository.SaveAsync();

            return new StandardResponse<UpdateGroupTaskDTO>()
            {
                Data = requestUpdateGroupTask,
                ServiceCode = ServiceCode.GroupTaskUpdated
            };
        }

        public async Task<BaseResponse<bool>> DeleteGroupTask(Guid groupTaskId, Guid adminId)
        {
            var deletedTask = await _groupTaskRepository.GetAll().FirstOrDefaultAsync(x => x.Id == groupTaskId );

            if (deletedTask is null)
                return new StandardResponse<bool> { Message = "Task not exists", ServiceCode = ServiceCode.EntityNotFound };
            if (deletedTask.Status <= StatusTask.MissedDate)
                return new StandardResponse<bool> { Message = "The task was not deleted because the task has the status process", ServiceCode = ServiceCode.UserNotAccess };
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.IdGroup == deletedTask.GroupId && x.AccountId == adminId && x.RoleAccount > RoleAccount.Default))
                return new StandardResponse<bool>{ Message = "You are not in this group or you do not have access", ServiceCode = ServiceCode.UserNotAccess };

            var Result = _groupTaskRepository.Delete(deletedTask);
            await _groupTaskRepository.SaveAsync();

            return new StandardResponse<bool>()
            {
                Data = Result,
                ServiceCode = ServiceCode.GroupTaskDeleted
            };
        }

        public async Task<BaseResponse<int>> UpdateStatusInGroupTasks()
        {
            var nowDate = DateTimeOffset.UtcNow.ToOffsetDateTime().Date;
            int countUpdatedTask = await _groupTaskRepository.GetAll().Where(x => x.DateEndWork.Date < nowDate && x.Status == StatusTask.Process).ExecuteUpdateAsync(x => x.SetProperty(prop => prop.Status, StatusTask.MissedDate));

            return new StandardResponse<int> { Data = countUpdatedTask, ServiceCode = ServiceCode.GroupTaskUpdated };
        }


        public async Task<BaseResponse<ResponseTasksPage>> GetTasksPage(string nameTask, Guid userId, Guid groupId)
        {
            var responseFullAccountStatusGroup = await _accountStatusGroupRepository.GetAll()
                                                                                    .Where(x => x.AccountId == userId && x.Group!.Id == groupId)
                                                                                    .Include(x => x.Group!.GroupTasks)
                                                                                    .Include(x => x.Group!.AccountStatusGroups)
                                                                                    .SingleOrDefaultAsync();

            if (responseFullAccountStatusGroup is null)
                return new StandardResponse<ResponseTasksPage> { Message = "Your status in group not found", ServiceCode = ServiceCode.UserNotAccess };

            var usersIdInGroup = responseFullAccountStatusGroup.Group!.AccountStatusGroups.Select(x => x.AccountId).ToArray();

            var responseAnotherApi = await _grpcService.GetUsers(usersIdInGroup);
            if (responseAnotherApi.ServiceCode != ServiceCode.GrpcUsersReadied)
                return new StandardResponse<ResponseTasksPage> { Message = responseAnotherApi.Message, ServiceCode = responseAnotherApi.ServiceCode };

            var tasksFromGroup = responseFullAccountStatusGroup.Group.GroupTasks.Where(x => x.Name.ToLower().Contains(nameTask.ToLower())).ToList();
            var isAdmin = responseFullAccountStatusGroup.RoleAccount > RoleAccount.Default;

            var response = await CreateTasks(tasksFromGroup, responseAnotherApi.Data.Users.ToArray(), isAdmin);
            return response;
        }

        private async Task<BaseResponse<ResponseTasksPage>> CreateTasks(List<GroupTask> groupTasks, User[] usersLoginWithId, bool isAdmin)
        {
            ResponseStageGroupTaskIcon[] stageGroupTask;
            ResponseGroupTaskView[] tasksPages = new ResponseGroupTaskView[groupTasks.Count];
            for(int i = 0; i < groupTasks.Count; i++)
            {
                // Add extensions methods
                var filt = Builders<StageGroupTask>.Filter.Where(x => x.IdGroupTask == groupTasks[i].Id);
                var proj = Builders<StageGroupTask>.Projection.Include(x =>  x.Id).Include(x => x.Name);

                stageGroupTask =  (await _stageGroupTaskRepository.GetCollection() // extensions methods
                                                                  .Find(filt)
                                                                  .Project(proj)
                                                                  .ToListAsync())
                                                                  .Select(x => new ResponseStageGroupTaskIcon(x["_id"].AsObjectId, x["Name"].AsString)) // extensions method 
                                                                  .ToArray();
                
                var namesUser = groupTasks[i].Team.Join(
                                        usersLoginWithId,
                                        userId => userId,
                                        loginWithIdUser => Guid.Parse(loginWithIdUser.Id),
                                        (task, loginWithIdUser) => loginWithIdUser.Login).ToArray();

                var groupTaskViewDTO = new ResponseGroupTaskView(groupTasks[i], namesUser, stageGroupTask);
                tasksPages[i] = groupTaskViewDTO;
            }

            if (tasksPages.Length == 0)
                return new StandardResponse<ResponseTasksPage> { Message = "Tasks not found", ServiceCode = ServiceCode.EntityNotFound };

            var tasksPageDTO = new ResponseTasksPage(tasksPages, isAdmin);

            return new StandardResponse<ResponseTasksPage>
            {
                Data = tasksPageDTO,
                ServiceCode = ServiceCode.GroupTaskReadied
            };
        }
    }
}
