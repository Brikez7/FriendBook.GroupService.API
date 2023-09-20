using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.DAL;
using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs;
using FriendBook.GroupService.API.Domain.DTO.StageGroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities.MongoDB;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.Response;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using NodaTime;
using NodaTime.Extensions;

namespace FriendBook.GroupService.API.BLL.Services
{
    public class StageGroupTaskService : IStageGroupTaskService
    {
        private readonly IStageGroupTaskRepository _stageGroupTaskRepository;
        private readonly IAccountStatusGroupRepository _accountStatusGroupRepository;
        public StageGroupTaskService(IStageGroupTaskRepository stageGroupTaskRepository, IAccountStatusGroupRepository accountStatusGroupRepository)
        {
            _stageGroupTaskRepository = stageGroupTaskRepository;
            _accountStatusGroupRepository = accountStatusGroupRepository;
        }

        public async Task<BaseResponse<ResponseStageGroupTaskView>> Create(RequestNewStageGroupTask entity, Guid userId, Guid groupId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == userId && groupId == x.IdGroup && x.RoleAccount > RoleAccount.Default)) 
                return new StandardResponse<ResponseStageGroupTaskView> { Message = "User not exist in this group", ServiceCode = ServiceCode.UserNotExists };

            if (await _stageGroupTaskRepository.GetCollection().Where(x => x.IdGroupTask == entity.IdGroupTask && x.Name == entity.Name).AnyAsync()) 
                return new StandardResponse<ResponseStageGroupTaskView> { Message = "Stage with name exists in this group", ServiceCode = ServiceCode.StageGroupTaskExists };

            var stageGroupTask = new StageGroupTask(entity.IdGroupTask,entity.Name, entity.Text, entity.DateCreate.ToOffsetDateTime(), entity.DateCreate.DateTime);
            var newEntity = await _stageGroupTaskRepository.AddAsync(stageGroupTask);

            var stageGroupTaskIcon = new ResponseStageGroupTaskView(newEntity.Id, newEntity.IdGroupTask,newEntity.Name, newEntity.Text, newEntity.DateUpdate, newEntity.DateCreate);
            return new StandardResponse<ResponseStageGroupTaskView> { Data = stageGroupTaskIcon, ServiceCode = ServiceCode.StageGroupTaskCreated };
        }

        public async Task<BaseResponse<bool>> Delete(ObjectId stageGroupTaskId, Guid userId, Guid groupId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == userId && groupId == x.IdGroup && x.RoleAccount > RoleAccount.Default))
                return new StandardResponse<bool> { Message = "User not exist in this group", ServiceCode = ServiceCode.UserNotExists };

            if (!await _stageGroupTaskRepository.GetCollection().Where(x => x.Id == stageGroupTaskId).AnyAsync())
                return new StandardResponse<bool> { Message = "Stage with name not exists in this group", ServiceCode = ServiceCode.StageGroupTaskExists };

            bool result = await _stageGroupTaskRepository.Delete(x => x.Id == stageGroupTaskId);

            return new StandardResponse<bool> { Data = result, ServiceCode = ServiceCode.StageGroupTaskDeleted };
        }

        public async Task<BaseResponse<ResponseStageGroupTaskView?>> GetStageGroupTaskById(ObjectId id, Guid userId, Guid groupId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == userId && groupId == x.IdGroup))
                return new StandardResponse<ResponseStageGroupTaskView?> { Message = "User not exist in this group", ServiceCode = ServiceCode.UserNotAccess };

            var stageGroupTask = await _stageGroupTaskRepository.GetCollection().Where(x => x.Id == id).FirstOrDefaultAsync();
            if (stageGroupTask == null)
                return new StandardResponse<ResponseStageGroupTaskView?> { ServiceCode = ServiceCode.EntityNotFound, Message = "Stage group task not found" };

            var stageGroupTaskDTO = new ResponseStageGroupTaskView(stageGroupTask.Id, stageGroupTask!.IdGroupTask, stageGroupTask.Name, stageGroupTask.Text, stageGroupTask.DateUpdate, stageGroupTask.DateCreate);

            return new StandardResponse<ResponseStageGroupTaskView> { Data = stageGroupTaskDTO, ServiceCode = ServiceCode.StageGroupTaskReadied}!; 
        }

        public ResponseStageGroupTaskIcon[] GetStagesGroupTaskIconByGroupId(Guid groupTaskId)
        {
            var stagesGroupTaskIcons = _stageGroupTaskRepository.GetCollection()
                                                .Where(x => x.IdGroupTask == groupTaskId)
                                                .SelectToDocument(x => new ResponseStageGroupTaskIcon(x.Id,x.Name))
                                                .Select(x => new ResponseStageGroupTaskIcon(x["_id"].AsObjectId, x["Name"].AsString)).ToArray();

            return stagesGroupTaskIcons;
        }

        public async Task<BaseResponse<StageGroupTaskDTO>> Update(StageGroupTaskDTO stageGroupTaskDTO, Guid userId, Guid groupId)
        {
            if (!await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == userId && groupId == x.IdGroup && x.RoleAccount > RoleAccount.Default))
                return new StandardResponse<StageGroupTaskDTO> { Message = "User not exist in this group", ServiceCode = ServiceCode.UserNotAccess };

            if (!await _stageGroupTaskRepository.GetCollection().Where(x => x.IdGroupTask == stageGroupTaskDTO.IdGroupTask).AnyAsync())
                return new StandardResponse<StageGroupTaskDTO> { Message = "Stage not exists in this group task", ServiceCode = ServiceCode.StageGroupTaskExists };


            OffsetDateTime now = DateTimeOffset.UtcNow.ToOffsetDateTime();

            var filter = Builders<StageGroupTask>.Filter.Where(x => x.Id == stageGroupTaskDTO.StageId);
            var updater = Builders<StageGroupTask>.Update.Set(x => x.DateUpdate, now)
                                                         .Set(x => x.Text, stageGroupTaskDTO.Text);

            var result = await _stageGroupTaskRepository.Update(filter, updater);

            return new StandardResponse<StageGroupTaskDTO> { Data = stageGroupTaskDTO, ServiceCode = ServiceCode.StageGroupTaskUpdated };
        }
    }
}
