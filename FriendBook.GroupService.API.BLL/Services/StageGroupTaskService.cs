using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.DAL.Repositories.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities.MongoDB;
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

        public async Task<BaseResponse<StageGroupTaskIconDTO>> Create(RequestStageGroupTasNew entity, Guid userId)
        {
            if (await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == userId && entity.IdGroupTask == x.IdGroup && x.RoleAccount > Domain.Entities.Postgres.RoleAccount.Default)) 
            {
                return new StandartResponse<StageGroupTaskIconDTO> { Message = "User not exist in this group", StatusCode = StatusCode.UserNotExists };
            }

            if (await _stageGroupTaskRepository.GetAll().AnyAsync(x => x.IdGroupTask == entity.IdGroupTask && x.Name == entity.Name)) 
            { 
                return new StandartResponse<StageGroupTaskIconDTO> { Message = "Stage with name exists in this group", StatusCode = StatusCode.StageGroupTaskExists };
            }

            DateTime now = DateTime.UtcNow;
            StageGroupTask stageGroupTask = new StageGroupTask(entity.IdGroupTask,entity.Name,"", now, now);
            var newEntity = await _stageGroupTaskRepository.AddAsync(stageGroupTask);

            StageGroupTaskIconDTO stageGroupTaskIcon = new StageGroupTaskIconDTO(newEntity.Id,newEntity.Name,newEntity.IdGroupTask);
            return new StandartResponse<StageGroupTaskIconDTO> { Data = stageGroupTaskIcon, StatusCode = StatusCode.StageGroupTaskCreate };
        }

        public async Task<BaseResponse<bool>> Delete(StageGroupTaskIconDTO entity, Guid userId)
        {
            if (await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == userId && entity.IdGroupTask == x.IdGroup && x.RoleAccount > Domain.Entities.Postgres.RoleAccount.Default))
            {
                return new StandartResponse<bool> { Message = "User not exist in this group", StatusCode = StatusCode.UserNotExists };
            }

            if (!await _stageGroupTaskRepository.GetAll().AnyAsync(x => x.IdGroupTask == entity.IdGroupTask && x.Name == entity.Name))
            {
                return new StandartResponse<bool> { Message = "Stage with name not exists in this group", StatusCode = StatusCode.StageGroupTaskExists };
            }

            bool result = await _stageGroupTaskRepository.Delete(x => x.Id == entity.StageGroupTaskId);

            return new StandartResponse<bool> { Data = result, StatusCode = StatusCode.StageGroupTaskDelete };
        }

        public async Task<BaseResponse<StageGroupTaskDTO?>> GetStageGroupTaskById(ObjectId id, Guid userId, Guid idGroupTask)
        {
            if (await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == userId && idGroupTask == x.IdGroup))
            {
                return new StandartResponse<StageGroupTaskDTO?> { Message = "User not exist in this group", StatusCode = StatusCode.UserNotExists };
            }

            var stageGroupTask = await _stageGroupTaskRepository.GetAll().FirstOrDefaultAsync(x => x.IdGroupTask == idGroupTask && x.Id == id);
            if (stageGroupTask == null)
            {
                return new StandartResponse<StageGroupTaskDTO?> { StatusCode= StatusCode.EntityNotFound, Message = "Stage group task not found" };
            }

            StageGroupTaskDTO stageGroupTaskDTO = new StageGroupTaskDTO(stageGroupTask!.IdGroupTask, stageGroupTask.Name, stageGroupTask.Text, stageGroupTask.DateUpdate, (DateTime)stageGroupTask.DateCreate!);

            return new StandartResponse<StageGroupTaskDTO> { Data = stageGroupTaskDTO, StatusCode = StatusCode.StageGroupTaskRead}!; 
        }

        public async Task<BaseResponse<List<StageGroupTaskIconDTO>>> GetStagesGroupTaskByGroupId(Guid groupId, Guid userId)
        {
            if (await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == userId && groupId == x.IdGroup))
            {
                return new StandartResponse<List<StageGroupTaskIconDTO>> { Message = "User not exist in this group", StatusCode = StatusCode.UserNotExists };
            }

            var list = await _stageGroupTaskRepository.GetAll().Select(x => new StageGroupTaskIconDTO(x.Id,x.Name,x.IdGroupTask)).ToListAsync();

            return new StandartResponse<List<StageGroupTaskIconDTO>> { Data = list, StatusCode = StatusCode.StageGroupTaskRead};
        }

        public async Task<BaseResponse<StageGroupTaskDTO>> Update(StageGroupTaskDTO stageGroupTaskDTO, Guid userId)
        {
            if (await _accountStatusGroupRepository.GetAll().AnyAsync(x => x.AccountId == userId && stageGroupTaskDTO.IdGroupTask == x.IdGroup && x.RoleAccount > Domain.Entities.Postgres.RoleAccount.Default))
            {
                return new StandartResponse<StageGroupTaskDTO> { Message = "User not exist in this group", StatusCode = StatusCode.UserNotExists };
            }

            if (!await _stageGroupTaskRepository.GetAll().AnyAsync(x => x.IdGroupTask == stageGroupTaskDTO.IdGroupTask && x.Name == stageGroupTaskDTO.Name))
            {
                return new StandartResponse<StageGroupTaskDTO> { Message = "Stage with name not exists in this group", StatusCode = StatusCode.StageGroupTaskExists };
            }

            DateTime now = DateTime.UtcNow;

            var filter = Builders<StageGroupTask>.Filter.Where(x => x.Id == stageGroupTaskDTO.StageGroupTaskId);
            var updater = Builders<StageGroupTask>.Update.Set(x =>  x.DateUpdate ,now)
                                                         .Set(x => x.Text, stageGroupTaskDTO.Text);
            var result = await _stageGroupTaskRepository.Update(filter, updater);

            if(result)
                return new StandartResponse<StageGroupTaskDTO> { Message = "Stage group task wasn`t updated", StatusCode = StatusCode.StageGroupTaskNotUpdated };

            return new StandartResponse<StageGroupTaskDTO> { Data = stageGroupTaskDTO, StatusCode = StatusCode.StageGroupTaskUpdate };
        }
    }
}
