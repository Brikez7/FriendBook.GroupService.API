using FriendBook.GroupService.API.BLL.Helpers;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs;
using FriendBook.GroupService.API.Domain.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using MongoDB.Bson;

namespace FriendBook.GroupService.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class StageGroupTaskController : ODataController
    {
        private readonly IStageGroupTaskService _stageGroupTaskService;
        private readonly IValidationService<RequestStageGroupTasNew> _requestStageGroupTasNewValidationService;
        private readonly IValidationService<StageGroupTaskDTO> _stageGroupTaskDTOValidationService;
        public Lazy<DataAccessToken> UserToken { get; set; }
        public StageGroupTaskController(IStageGroupTaskService stageGroupTaskService, IHttpContextAccessor httpContextAccessor,
            IValidationService<RequestStageGroupTasNew> validatorStageGroupTasNew, IValidationService<StageGroupTaskDTO> validatorStageGroupTaskDTO)
        {
            _stageGroupTaskService = stageGroupTaskService;
            UserToken = AccessTokenHelper.CreateUser(httpContextAccessor.HttpContext!.User.Claims);
            _requestStageGroupTasNewValidationService = validatorStageGroupTasNew;
            _stageGroupTaskDTOValidationService = validatorStageGroupTaskDTO;
        }

        [HttpPost("Create/{groupId}")]
        public async Task<IActionResult> CreateStageGroupTask([FromRoute] Guid groupId, [FromBody] RequestStageGroupTasNew requestStageGroupTasNew) 
        {
            var responseValidation = await _requestStageGroupTasNewValidationService.ValidateAsync(requestStageGroupTasNew);
            if (responseValidation.StatusCode != Domain.Response.Code.EntityIsValidated)
                return Ok(responseValidation);

            var stageGroupTaskIconDTO = await _stageGroupTaskService.Create(requestStageGroupTasNew, UserToken.Value.Id, groupId);
            return Ok(stageGroupTaskIconDTO);
        }
        [HttpPut("Update/{groupId}")]
        public async Task<IActionResult> UpdateStageGroupTask([FromRoute] Guid groupId, [FromBody] StageGroupTaskDTO stageGroupTaskDTO)
        {
            var responseValidation = await _stageGroupTaskDTOValidationService.ValidateAsync(stageGroupTaskDTO);
            if (responseValidation.StatusCode != Domain.Response.Code.EntityIsValidated)
                return Ok(responseValidation);

            var stageGroupTaskIconDTO = await _stageGroupTaskService.Update(stageGroupTaskDTO, UserToken.Value.Id, groupId);
            return Ok(stageGroupTaskIconDTO);
        }
        [HttpDelete("Delete/{groupId}")]
        public async Task<IActionResult> DeleteStageGroupTask([FromRoute] Guid groupId, [FromQuery] ObjectId stageGroupTaskId)
        {
            var result = await _stageGroupTaskService.Delete(stageGroupTaskId, UserToken.Value.Id, groupId);
            return Ok(stageGroupTaskId);
        }
        [HttpGet("Get/{groupId}")]
        public async Task<IActionResult> GetStageGroupTask([FromRoute] Guid groupId, [FromQuery] ObjectId stageGroupTaskId)
        {
            var stageGroupTaskIconDTO = await _stageGroupTaskService.GetStageGroupTaskById(stageGroupTaskId, UserToken.Value.Id, groupId);
            return Ok(stageGroupTaskIconDTO);
        }
    }
}
