using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.CustomClaims;
using FriendBook.GroupService.API.Domain.DTO;
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
    public class GroupController : ODataController
    {
        private readonly IGroupService _groupService;
        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpDelete("Delete/{idGroupGuid}")]
        public async Task<IActionResult> DeleteGroup([FromRoute] Guid idGroupGuid)
        {
            Guid userId = Guid.Parse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value);

            var response = await _groupService.DeleteGroup(idGroupGuid, userId);
            return Ok(response);
        }

        [HttpPost("Create/{groupName}")]
        public async Task<IActionResult> CreateGroup([FromRoute] string groupName)
        {
            Guid userId = Guid.Parse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value);

            try
            {
                BaseResponse<bool> responseAnotherAPI; 
                var reg_Req = new MyRequest($"https://localhost:7227/api/IdentityServer/checkUserExists?userId={userId}", null, null);
                await reg_Req.SendRequest(MyTypeRequest.GET);
                responseAnotherAPI = JsonConvert.DeserializeObject<StandartResponse<bool>>(reg_Req.Response);

                if (responseAnotherAPI.Message != null || !responseAnotherAPI.Data) 
                {
                    return Ok(new StandartResponse<GroupDTO> 
                    { 
                        Message = responseAnotherAPI.Message ?? "Account not exists",
                        StatusCode = Domain.StatusCode.InternalServerError
                    });
                }
            }
            catch (Exception e)
            {
                return Ok(new StandartResponse<GroupDTO>()
                {
                    Message = $"Identity server not responsing {e.Message}",
                    StatusCode = Domain.StatusCode.InternalServerError,
                });
            }
            var newGroup = new Group(groupName, userId);
            var response = await _groupService.CreateGroup(newGroup);

            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateGroup([FromBody] GroupDTO groupDTO)
        {
            Guid userId = Guid.Parse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value);

            var response = await _groupService.UpdateGroup(groupDTO, userId);
            return Ok(response);
        }

        [HttpGet("OData/GetMyGroups")]
        [EnableQuery]
        public async Task<IActionResult> GetMyGroups()
        {
            Guid userId = Guid.Parse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value);
            
            var response = await _groupService.GeyGroupsByUserId(userId);
            return Ok(response);
        }

        [HttpGet("OData/GetMyGroupsWithMyStatus")]
        [EnableQuery]
        public async Task<IActionResult> GetMyGroupsWithMyStatus()
        {
            Guid userId = Guid.Parse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value);
            
            var response = await _groupService.GetGroupsWithStatusByUserId(userId);
            return Ok(response);
        }
    }
}
