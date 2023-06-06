using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.CustomClaims;
using FriendBook.GroupService.API.Domain.DTO;
using FriendBook.GroupService.API.Domain.DTO.GroupTasksDTO;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;
using FriendBook.GroupService.API.Domain.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
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
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                var response = await _groupTaskService.DeleteGroupTask(groupTaskKey, userId);
                return Ok(response);
            }
            return Ok(new StandartResponse<bool>
            {
                Message = "Not valid token",
                StatusCode = Domain.StatusCode.InternalServerError
            });
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateGroupTask([FromBody] GroupTaskNewDTO newGroupTaskDTO)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                var response = await _groupTaskService.CreateGroupTask(newGroupTaskDTO,userId);
                return Ok(response);
            }
            return Ok(new StandartResponse<GroupTaskViewDTO>
            {
                Message = "Not valid token",
                StatusCode = Domain.StatusCode.InternalServerError
            });
        }


        [HttpPut("Update")]
        public async Task<IActionResult> UpdateGroupTask([FromBody] GroupTaskChangedDTO groupTaskDTO)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                var response = await _groupTaskService.UpdateGroupTask(groupTaskDTO, userId);

                if (response.Data != null)
                {
                    return Ok(new StandartResponse<GroupTaskViewDTO>
                    {
                        Data = new GroupTaskViewDTO(response.Data)
                    });
                }

                return Ok(response);
            }
            return Ok(new StandartResponse<GroupTaskViewDTO>
            {
                Message = "Not valid token",
                StatusCode = Domain.StatusCode.InternalServerError
            });
        }

        [HttpPut("SubscribeTask")]
        public async Task<IActionResult> SubscribeTask([FromBody] GroupTaskKeyDTO groupTaskKeyDTO)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                var response = await _groupTaskService.SubcsribeGroupTask(groupTaskKeyDTO,userId);

                return Ok(response);
            }
            return Ok(new StandartResponse<GroupTask>
            {
                StatusCode = Domain.StatusCode.IdNotFound,
                Message = "Not valid token",
            });
        }
        [HttpPut("UnsubscribeTask")]
        public async Task<IActionResult> UnsubscribeTask([FromBody] GroupTaskKeyDTO groupTaskKeyDTO)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                var response = await _groupTaskService.UnsubcsribeGroupTask(groupTaskKeyDTO, userId);
                return Ok(response);
            }
            return Ok(new StandartResponse<GroupTask>
            {
                StatusCode = Domain.StatusCode.IdNotFound,
                Message = "Not valid token",
            });
        }
        [HttpGet("OData/GetTasks")]
        [EnableQuery]
        public async Task<IActionResult> GetTasksByNameTaskAndIdGroup([FromQuery] Guid idGroup, [FromQuery] string nameTask = "")
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
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
                Ok(response);
            }
            return Ok(new StandartResponse<TasksPageDTO>
            {
                StatusCode = Domain.StatusCode.IdNotFound,
                Message = "Id not found or user not outorisation"
            });
        }
    }
}
