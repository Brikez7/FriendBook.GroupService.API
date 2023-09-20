using FriendBook.GroupService.API.BLL.Helpers;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendBook.GroupService.API.Controllers
{
    [ApiController]
    [Route("GroupService/v1/[controller]")]
    [Authorize]
    public class GroupTaskController : ControllerBase
    {
        private readonly IGroupTaskService _groupTaskService;
        private readonly IValidationService<RequestNewGroupTask> _groupTaskNewValidationService;
        private readonly IValidationService<UpdateGroupTaskDTO> _groupTaskChangedValidationService;
        public Lazy<AccessToken> UserToken { get; set; }
        public GroupTaskController(IGroupTaskService groupTaskService, IValidationService<RequestNewGroupTask> requestGroupTaskNewValidationService,
             IValidationService<UpdateGroupTaskDTO> requestGroupTaskChangedValidationService,IHttpContextAccessor httpContextAccessor)
        {
            _groupTaskService = groupTaskService;
            _groupTaskNewValidationService = requestGroupTaskNewValidationService;
            _groupTaskChangedValidationService = requestGroupTaskChangedValidationService;
            UserToken = AccessTokenHelper.CreateUser(httpContextAccessor.HttpContext!.User.Claims);
        }

        [HttpDelete("Delete/{GroupTaskId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid GroupTaskId)
        {
            var response = await _groupTaskService.DeleteGroupTask(GroupTaskId, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] RequestNewGroupTask newGroupTaskDTO)
        {
            var responseValidation = await _groupTaskNewValidationService.ValidateAsync(newGroupTaskDTO);
            if (responseValidation.ServiceCode != ServiceCode.EntityIsValidated)
                return Ok(responseValidation);

            var response = await _groupTaskService.CreateGroupTask(newGroupTaskDTO,UserToken.Value.Id, UserToken.Value.Login);
            return Ok(response);
        }


        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateGroupTaskDTO groupTaskDTO)
        {
            var responseValidation = await _groupTaskChangedValidationService.ValidateAsync(groupTaskDTO);
            if (responseValidation.ServiceCode != ServiceCode.EntityIsValidated)
                return Ok(responseValidation);

            var response = await _groupTaskService.UpdateGroupTask(groupTaskDTO, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpPut("SubscribeTask/{GroupTaskId}")]
        public async Task<IActionResult> SubscribeTask([FromRoute] Guid GroupTaskId)
        {
            var response = await _groupTaskService.SubscribeGroupTask(GroupTaskId, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpPut("UnsubscribeTask/{GroupTaskId}")]
        public async Task<IActionResult> UnsubscribeTask([FromRoute] Guid GroupTaskId)
        {
            var response = await _groupTaskService.UnsubscribeGroupTask(GroupTaskId, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpGet("GetMyTasksByNameAndGroupId/{groupId}")]
        public async Task<IActionResult> GetMyTasksByNameAndGroupId([FromRoute] Guid groupId, [FromQuery] string nameTask = "")
        {
            var response = await _groupTaskService.GetTasksPage(nameTask, UserToken.Value.Id, groupId);
            return Ok(response);
        }
    }
}
