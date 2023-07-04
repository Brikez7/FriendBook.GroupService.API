using FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Response;
using MongoDB.Bson;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IStageGroupTaskService
    {
        public Task<BaseResponse<StageGroupTaskIconDTO>> Create(RequestStageGroupTasNew requestStageGroupTasNew, Guid userId);
        public Task<BaseResponse<StageGroupTaskDTO>> Update(StageGroupTaskDTO stageGroupTaskDTO, Guid userId);
        public Task<BaseResponse<bool>> Delete(StageGroupTaskIconDTO stageGroupTaskIconDTO, Guid userId);
        public Task<BaseResponse<List<StageGroupTaskIconDTO>>> GetStagesGroupTaskByGroupId(Guid groupid, Guid userId);
        public Task<BaseResponse<StageGroupTaskDTO>> GetStageGroupTaskById(ObjectId id, Guid userId, Guid groupId);
    }
}
