using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities.MongoDB;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.Response;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

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

        public async Task<BaseResponse<ResponseStageGroupTaskIcon>> Create(RequestStageGroupTasNew entity, Guid userId, Guid groupId)
        {
            if (await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == userId && groupId == x.IdGroup && x.RoleAccount > RoleAccount.Default)) 
                return new StandardResponse<ResponseStageGroupTaskIcon> { Message = "User not exist in this group", ServiceCode = ServiceCode.UserNotExists };


            if (await _stageGroupTaskRepository.GetAll().AnyAsync(x => x.IdGroupTask == entity.IdGroupTask && x.Name == entity.Name)) 
                return new StandardResponse<ResponseStageGroupTaskIcon> { Message = "Stage with name exists in this group", ServiceCode = ServiceCode.StageGroupTaskExists };

            DateTime now = DateTime.UtcNow;
            StageGroupTask stageGroupTask = new StageGroupTask(entity.IdGroupTask,entity.Name,"", now, now);
            var newEntity = await _stageGroupTaskRepository.AddAsync(stageGroupTask);

            ResponseStageGroupTaskIcon stageGroupTaskIcon = new(newEntity.Id,newEntity.Name,newEntity.IdGroupTask);
            return new StandardResponse<ResponseStageGroupTaskIcon> { Data = stageGroupTaskIcon, ServiceCode = ServiceCode.StageGroupTaskCreated };
        }

        public async Task<BaseResponse<bool>> Delete(ObjectId stageGroupTaskId, Guid userId, Guid groupId)
        {
            if (await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == userId && groupId == x.IdGroup && x.RoleAccount > RoleAccount.Default))
                return new StandardResponse<bool> { Message = "User not exist in this group", ServiceCode = ServiceCode.UserNotExists };

            if (!await _stageGroupTaskRepository.GetAll().AnyAsync(x => x.Id == stageGroupTaskId))
                return new StandardResponse<bool> { Message = "Stage with name not exists in this group", ServiceCode = ServiceCode.StageGroupTaskExists };

            bool result = await _stageGroupTaskRepository.Delete(x => x.Id == stageGroupTaskId);

            return new StandardResponse<bool> { Data = result, ServiceCode = ServiceCode.StageGroupTaskDeleted };
        }

        public async Task<BaseResponse<StageGroupTaskDTO?>> GetStageGroupTaskById(ObjectId id, Guid userId, Guid idGroupTask)
        {
            if (await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == userId && idGroupTask == x.IdGroup))
                return new StandardResponse<StageGroupTaskDTO?> { Message = "User not exist in this group", ServiceCode = ServiceCode.UserNotExists };

            var stageGroupTask = await _stageGroupTaskRepository.GetAll().FirstOrDefaultAsync(x => x.IdGroupTask == idGroupTask && x.Id == id);
            if (stageGroupTask == null)
                return new StandardResponse<StageGroupTaskDTO?> { ServiceCode= ServiceCode.EntityNotFound, Message = "Stage group task not found" };

            StageGroupTaskDTO stageGroupTaskDTO = new StageGroupTaskDTO(stageGroupTask!.IdGroupTask, stageGroupTask.Name, stageGroupTask.Text, stageGroupTask.DateUpdate, (DateTime)stageGroupTask.DateCreate!);

            return new StandardResponse<StageGroupTaskDTO> { Data = stageGroupTaskDTO, ServiceCode = ServiceCode.StageGroupTaskReadied}!; 
        }

        public async Task<BaseResponse<List<ResponseStageGroupTaskIcon>>> GetStagesGroupTaskIconByGroupId(Guid groupId, Guid userId)
        {
            if (await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == userId && groupId == x.IdGroup))
                return new StandardResponse<List<ResponseStageGroupTaskIcon>> { Message = "User not exist in this group", ServiceCode = ServiceCode.UserNotExists };

            var list = await _stageGroupTaskRepository.GetAll().Select(x => new ResponseStageGroupTaskIcon(x.Id,x.Name,x.IdGroupTask)).ToListAsync();

            return new StandardResponse<List<ResponseStageGroupTaskIcon>> { Data = list, ServiceCode = ServiceCode.StageGroupTaskReadied};
        }

        public async Task<BaseResponse<StageGroupTaskDTO>> Update(StageGroupTaskDTO stageGroupTaskDTO, Guid userId, Guid groupId)
        {
            if (await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == userId && groupId == x.IdGroup && x.RoleAccount > RoleAccount.Default))
                return new StandardResponse<StageGroupTaskDTO> { Message = "User not exist in this group", ServiceCode = ServiceCode.UserNotExists };


            if (!await _stageGroupTaskRepository.GetAll().AnyAsync(x => x.IdGroupTask == stageGroupTaskDTO.IdGroupTask && x.Name == stageGroupTaskDTO.Name))
                return new StandardResponse<StageGroupTaskDTO> { Message = "Stage with name not exists in this group", ServiceCode = ServiceCode.StageGroupTaskExists };


            DateTime now = DateTime.UtcNow;

            var filter = Builders<StageGroupTask>.Filter.Where(x => x.Id == stageGroupTaskDTO.StageGroupTaskId);
            var updater = Builders<StageGroupTask>.Update.Set(x =>  x.DateUpdate ,now)
                                                         .Set(x => x.Text, stageGroupTaskDTO.Text);

            var result = await _stageGroupTaskRepository.Update(filter, updater);

            return new StandardResponse<StageGroupTaskDTO> { Data = stageGroupTaskDTO, ServiceCode = ServiceCode.StageGroupTaskUpdated };
        }
    }
}
