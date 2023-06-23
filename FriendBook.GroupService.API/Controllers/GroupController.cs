using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.CustomClaims;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
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
    public class GroupController : ODataController
    {
        private readonly IGroupService _groupService;
        private readonly IValidationService<GroupDTO> _groupDTOValidationService;
        public Lazy<UserTokenAuth> UserToken { get; set; }
        public GroupController(IGroupService groupService, IValidationService<GroupDTO> validationService)
        {
            _groupService = groupService;
            _groupDTOValidationService = validationService;
            UserToken = new Lazy<UserTokenAuth>(() => UserTokenAuth.CreateUserToken(User.Claims));
        }

        [HttpDelete("Delete/{idGroupGuid}")]
        public async Task<IActionResult> DeleteGroup([FromRoute] Guid idGroupGuid)
        {
            var response = await _groupService.DeleteGroup(idGroupGuid, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpPost("Create/{groupName}")]
        public async Task<IActionResult> CreateGroup([FromRoute] string groupName)
        {
            try
            {
                BaseResponse<bool> responseAnotherAPI; 
                var reg_Req = new MyRequest($"https://localhost:7227/api/IdentityServer/checkUserExists?userId={UserToken.Value.Id}", null, null);
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
            var newGroup = new Group(groupName, UserToken.Value.Id);
            var response = await _groupService.CreateGroup(newGroup);

            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateGroup([FromBody] GroupDTO groupDTO)
        {
            var responseValidation = await _groupDTOValidationService.ValidateAsync(groupDTO);
            if (responseValidation is not null)
                return Ok(responseValidation);

            var response = await _groupService.UpdateGroup(groupDTO, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpGet("OData/GetMyGroups")]
        [EnableQuery]
        public async Task<IActionResult> GetMyGroups()
        {
            var response = await _groupService.GeyGroupsByUserId(UserToken.Value.Id);
            return Ok(response);
        }

        [HttpGet("OData/GetMyGroupsWithMyStatus")]
        [EnableQuery]
        public async Task<IActionResult> GetMyGroupsWithMyStatus()
        {
            var response = await _groupService.GetGroupsWithStatusByUserId(UserToken.Value.Id);
            return Ok(response);
        }
    }
}
