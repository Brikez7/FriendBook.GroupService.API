using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;
using FriendBook.GroupService.API.Domain.Requests;
using FriendBook.GroupService.API.Domain.UserToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Newtonsoft.Json;

namespace FriendBook.GroupService.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class GroupTaskController : ODataController
    {
        private readonly IGroupTaskService _groupTaskService;
        private readonly IAccountStatusGroupService _accountStatusGroupService;
        private readonly IValidationService<RequestGroupTaskNew> _requestGroupTaskNewValidationService;
        private readonly IValidationService<RequestGroupTaskKey> _requestGroupTaskKeyValidationService;
        private readonly IValidationService<RequestGroupTaskChanged> _requestGroupTaskChangedValidationService;
        public Lazy<UserTokenAuth> UserToken { get; set; }

        public GroupTaskController(IGroupTaskService groupTaskService, IAccountStatusGroupService accountStatusGroupService,
            IValidationService<RequestGroupTaskNew> requestGroupTaskNewValidationService, IValidationService<RequestGroupTaskKey> requestGroupTaskKeyValidationService,
            IValidationService<RequestGroupTaskChanged> requestGroupTaskChangedValidationService)
        {
            _groupTaskService = groupTaskService;
            _accountStatusGroupService = accountStatusGroupService;
            _requestGroupTaskNewValidationService = requestGroupTaskNewValidationService;
            _requestGroupTaskKeyValidationService = requestGroupTaskKeyValidationService;
            _requestGroupTaskChangedValidationService = requestGroupTaskChangedValidationService;

            UserToken = new Lazy<UserTokenAuth>(() => UserTokenAuth.CreateUserToken(User.Claims));
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteGroupTask([FromBody] RequestGroupTaskKey groupTaskKey)
        {
            var responseValidation = await _requestGroupTaskKeyValidationService.ValidateAsync(groupTaskKey);
            if (responseValidation is not null)
                return Ok(responseValidation);

            var response = await _groupTaskService.DeleteGroupTask(groupTaskKey, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateGroupTask([FromBody] RequestGroupTaskNew newGroupTaskDTO)
        {
            var responseValidation = await _requestGroupTaskNewValidationService.ValidateAsync(newGroupTaskDTO);
            if (responseValidation is not null)
                return Ok(responseValidation);

            var response = await _groupTaskService.CreateGroupTask(newGroupTaskDTO,UserToken.Value.Id, UserToken.Value.Login);
            return Ok(response);
        }


        [HttpPut("Update")]
        public async Task<IActionResult> UpdateGroupTask([FromBody] RequestGroupTaskChanged groupTaskDTO)
        {
            var responseValidation = await _requestGroupTaskChangedValidationService.ValidateAsync(groupTaskDTO);
            if (responseValidation is not null)
                return Ok(responseValidation);

            var response = await _groupTaskService.UpdateGroupTask(groupTaskDTO, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpPut("SubscribeTask")]
        public async Task<IActionResult> SubscribeTask([FromBody] RequestGroupTaskKey groupTaskKeyDTO)
        {
            var responseValidation = await _requestGroupTaskKeyValidationService.ValidateAsync(groupTaskKeyDTO);
            if (responseValidation is not null)
                return Ok(responseValidation);

            var response = await _groupTaskService.SubcsribeGroupTask(groupTaskKeyDTO,UserToken.Value.Id);
            return Ok(response);
        }
        [HttpPut("UnsubscribeTask")]
        public async Task<IActionResult> UnsubscribeTask([FromBody] RequestGroupTaskKey groupTaskKeyDTO)
        {
            var responseValidation = await _requestGroupTaskKeyValidationService.ValidateAsync(groupTaskKeyDTO);
            if (responseValidation is not null)
                return Ok(responseValidation);

            var response = await _groupTaskService.UnsubcsribeGroupTask(groupTaskKeyDTO, UserToken.Value.Id);
            return Ok(response);
        }
        [HttpGet("OData/GetTasks")]
        [EnableQuery]
        public async Task<IActionResult> GetTasksByNameTaskAndIdGroup([FromQuery] Guid idGroup, [FromQuery] string nameTask = "")
        {
            var responseAccountStatusGroup = await _accountStatusGroupService.GetAccountStatusGroupByIdGroupAndUserId(UserToken.Value.Id, idGroup);

            if(responseAccountStatusGroup.Data is null) 
            {
                return Ok(responseAccountStatusGroup);
            }

            var usersIdFromGroup = responseAccountStatusGroup.Data.Group.AccountStatusGroups.Select(x => x.AccountId).ToArray();
            var tasksFromGroup = responseAccountStatusGroup.Data.Group.GroupTasks.Where(x => x.Name.ToLower().Contains(nameTask.ToLower())).ToList();
            var isAdmin = responseAccountStatusGroup.Data.RoleAccount > RoleAccount.Default;

            var jsonUsersId = JsonConvert.SerializeObject(usersIdFromGroup);
            BaseResponse<Tuple<Guid, string>[]> responseUsersLoginWithId;// New sservice
            try
            {
                var reg_Req = new MyRequest($"https://localhost:7227/api/IdentityServer/getLoginsUsers",null, jsonUsersId);
                await reg_Req.SendRequest(MyTypeRequest.POST);
                responseUsersLoginWithId = JsonConvert.DeserializeObject<StandartResponse<Tuple<Guid, string>[]>>(reg_Req.Response);
            }
            catch (Exception e)
            {
                return Ok(new StandartResponse<AccountStatusGroup>()
                {
                    Message = $"Identity server not responsing {e.Message}",
                    StatusCode = Domain.StatusCode.InternalServerError,
                });
            }//

            var response = _accountStatusGroupService.TasksJoinUsersLoginWithId(tasksFromGroup, responseUsersLoginWithId.Data, isAdmin);
            return Ok(response);
        }
    }
}
