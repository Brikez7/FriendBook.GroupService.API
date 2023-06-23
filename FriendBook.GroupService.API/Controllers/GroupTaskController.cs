using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.CustomClaims;
using FriendBook.GroupService.API.Domain.DTO.GroupTasksDTO;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;
using FriendBook.GroupService.API.Domain.Requests;
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
        public GroupTaskController(IGroupTaskService groupTaskService, IAccountStatusGroupService accountStatusGroupService)
        {
            _groupTaskService = groupTaskService;
            _accountStatusGroupService = accountStatusGroupService;
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteGroupTask([FromBody] GroupTaskKeyDTO groupTaskKey)
        {
            Guid userId = Guid.Parse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value);
            
            var response = await _groupTaskService.DeleteGroupTask(groupTaskKey, userId);
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateGroupTask([FromBody] GroupTaskNewDTO newGroupTaskDTO)
        {
            Guid userId = Guid.Parse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value);
            string login = User.Claims.First(x => x.Type == CustomClaimType.Login).Value;

            var response = await _groupTaskService.CreateGroupTask(newGroupTaskDTO,userId, login);
            return Ok(response);
        }


        [HttpPut("Update")]
        public async Task<IActionResult> UpdateGroupTask([FromBody] GroupTaskChangedDTO groupTaskDTO)
        {
            Guid userId = Guid.Parse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value);
            
            var response = await _groupTaskService.UpdateGroupTask(groupTaskDTO, userId);
            return Ok(response);
        }

        [HttpPut("SubscribeTask")]
        public async Task<IActionResult> SubscribeTask([FromBody] GroupTaskKeyDTO groupTaskKeyDTO)
        {
            Guid userId = Guid.Parse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value);
            
            var response = await _groupTaskService.SubcsribeGroupTask(groupTaskKeyDTO,userId);
            return Ok(response);
        }
        [HttpPut("UnsubscribeTask")]
        public async Task<IActionResult> UnsubscribeTask([FromBody] GroupTaskKeyDTO groupTaskKeyDTO)
        {
            Guid userId = Guid.Parse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value);
            
            var response = await _groupTaskService.UnsubcsribeGroupTask(groupTaskKeyDTO, userId);
            return Ok(response);
        }
        [HttpGet("OData/GetTasks")]
        [EnableQuery]
        public async Task<IActionResult> GetTasksByNameTaskAndIdGroup([FromQuery] Guid idGroup, [FromQuery] string nameTask = "")
        {
            Guid userId = Guid.Parse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value);
            var responseAccountStatusGroup = await _accountStatusGroupService.GetAccountStatusGroupByIdGroupAndUserId(userId, idGroup);

            if(responseAccountStatusGroup.Message != null) 
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
            }

            var response = _accountStatusGroupService.TasksJoinUsersLoginWithId(tasksFromGroup, responseUsersLoginWithId.Data, isAdmin);
            return Ok(response);
        }
    }
}
