using FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Response;
using MongoDB.Bson;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IStageGroupTaskService
    {
        public Task<BaseResponse<ResponseStageGroupTaskIcon>> Create(RequestStageGroupTasNew requestStageGroupTasNew, Guid adminId, Guid groupId);
        public Task<BaseResponse<StageGroupTaskDTO>> Update(StageGroupTaskDTO stageGroupTaskDTO, Guid adminId, Guid groupId);
        public Task<BaseResponse<bool>> Delete(ObjectId stageGroupTaskId, Guid adminId, Guid groupId);
        public Task<BaseResponse<ResponseStageGroupTaskIcon[]>> GetStagesGroupTaskIconByGroupId(Guid groupid, Guid userId);
        public Task<BaseResponse<StageGroupTaskDTO?>> GetStageGroupTaskById(ObjectId stageGroupTaskId, Guid userId, Guid groupId);
    }
}
