using FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs;
using FriendBook.GroupService.API.Domain.DTO.StageGroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Response;
using MongoDB.Bson;

namespace FriendBook.GroupService.API.BLL.Interfaces
{
    public interface IStageGroupTaskService
    {
        public Task<BaseResponse<ResponseStageGroupTaskView>> Create(RequestNewStageGroupTask requestStageGroupTasNew, Guid adminId, Guid groupId);
        public Task<BaseResponse<StageGroupTaskDTO>> Update(StageGroupTaskDTO stageGroupTaskDTO, Guid adminId, Guid groupId);
        public Task<BaseResponse<bool>> Delete(ObjectId stageGroupTaskId, Guid adminId, Guid groupId);
        public ResponseStageGroupTaskIcon[] GetStagesGroupTaskIconByGroupId(Guid groupTaskId);
        public Task<BaseResponse<ResponseStageGroupTaskView?>> GetStageGroupTaskById(ObjectId stageGroupTaskId, Guid userId, Guid groupId);
    }
}
