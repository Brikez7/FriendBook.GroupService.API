﻿using FriendBook.GroupService.API.BLL.Helpers;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.DocumentGroupTaskDTOs;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace FriendBook.GroupService.API.Controllers
{
    [ApiController]
    [Route("GroupService/v1/[controller]")]
    [Authorize]
    public class StageGroupTaskController : ControllerBase
    {
        private readonly IStageGroupTaskService _stageGroupTaskService;
        private readonly IValidationService<RequestNewStageGroupTask> _requestStageGroupTasNewValidationService;
        private readonly IValidationService<UpdateStageGroupTaskDTO> _stageGroupTaskDTOValidationService;
        public Lazy<DataAccessToken> UserToken { get; set; }
        public StageGroupTaskController(IStageGroupTaskService stageGroupTaskService, IHttpContextAccessor httpContextAccessor,
            IValidationService<RequestNewStageGroupTask> validatorStageGroupTasNew, IValidationService<UpdateStageGroupTaskDTO> validatorStageGroupTaskDTO)
        {
            _stageGroupTaskService = stageGroupTaskService;
            UserToken = AccessTokenHelper.CreateUser(httpContextAccessor.HttpContext!.User.Claims);
            _requestStageGroupTasNewValidationService = validatorStageGroupTasNew;
            _stageGroupTaskDTOValidationService = validatorStageGroupTaskDTO;
        }

        [HttpPost("Create/{groupId}")]
        public async Task<IActionResult> Create([FromRoute] Guid groupId, [FromBody] RequestNewStageGroupTask requestStageGroupTasNew) 
        {
           var responseValidation = await _requestStageGroupTasNewValidationService.ValidateAsync(requestStageGroupTasNew);
            if (responseValidation.ServiceCode != ServiceCode.EntityIsValidated)
                return Ok(responseValidation);

            var stageGroupTaskIconDTO = await _stageGroupTaskService.Create(requestStageGroupTasNew, UserToken.Value.Id, groupId);
            return Ok(stageGroupTaskIconDTO);
        }

        [HttpPut("Update/{groupId}")]
        public async Task<IActionResult> UpdateStageGroupTask([FromRoute] Guid groupId, [FromBody] UpdateStageGroupTaskDTO stageGroupTaskDTO)
        {
            var responseValidation = await _stageGroupTaskDTOValidationService.ValidateAsync(stageGroupTaskDTO);
            if (responseValidation.ServiceCode != ServiceCode.EntityIsValidated)
                return Ok(responseValidation);

            var updatedStageGroupTaskDTO = await _stageGroupTaskService.Update(stageGroupTaskDTO, UserToken.Value.Id, groupId);
            return Ok(updatedStageGroupTaskDTO);
        }

        [HttpDelete("Delete/{groupId}")]
        public async Task<IActionResult> DeleteStageGroupTask([FromRoute] Guid groupId, [FromQuery] ObjectId stageGroupTaskId)
        {
            var result = await _stageGroupTaskService.Delete(stageGroupTaskId, UserToken.Value.Id, groupId);
            return Ok(result);
        }

        [HttpGet("Get/{groupId}")]
        public async Task<IActionResult> GetStageGroupTask([FromRoute] Guid groupId, [FromQuery] ObjectId stageGroupTaskId)
        {
            var stageGroupTaskIconDTO = await _stageGroupTaskService.GetStageGroupTaskById(stageGroupTaskId, UserToken.Value.Id, groupId);
            return Ok(stageGroupTaskIconDTO);
        }
    }
}
