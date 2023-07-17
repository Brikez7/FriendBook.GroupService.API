using FriendBook.GroupService.API.BLL.gRPCClients.AccountService;
using FriendBook.GroupService.API.BLL.GrpcServices;
using FriendBook.GroupService.API.BLL.Helpers;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FriendBook.GroupService.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class GroupController : ODataController
    {
        private readonly IContactGroupService _groupService;
        private readonly IValidationService<RequestGroupUpdate> _groupDTOValidationService;
        private readonly IGrpcService _grpcIdentityService;
        public Lazy<DataAccessToken> UserToken { get; set; }
        public GroupController(IContactGroupService groupService, IValidationService<RequestGroupUpdate> validationService, IGrpcService grpcService, IHttpContextAccessor httpContext)
        {
            _groupService = groupService;
            _groupDTOValidationService = validationService;
            UserToken = AccessTokenHelper.CreateUser(httpContext.HttpContext!.User.Claims);
            _grpcIdentityService = grpcService;
        }

        [HttpDelete("Delete/{groupId}")]
        public async Task<IActionResult> DeleteGroup([FromRoute] Guid groupId)
        {
            var response = await _groupService.DeleteGroup(groupId, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpPost("Create/{groupName}")]
        public async Task<IActionResult> CreateGroup([FromRoute] string groupName)
        {
            BaseResponse<ResponseUserExists> responseAnotherAPI = await _grpcIdentityService.CheckUserExists(UserToken.Value.Id);
            if (responseAnotherAPI.StatusCode != Domain.Response.StatusCode.UserExists)
                return Ok(responseAnotherAPI);

            var response = await _groupService.CreateGroup(groupName, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateGroup([FromBody] RequestGroupUpdate requestGroupUpdate)
        {
            var responseValidation = await _groupDTOValidationService.ValidateAsync(requestGroupUpdate);
            if (responseValidation.StatusCode == Domain.Response.StatusCode.ErrorValidation)
                return Ok(responseValidation);

            var response = await _groupService.UpdateGroup(requestGroupUpdate, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpGet("OData/Get")]
        [EnableQuery]
        public async Task<IActionResult> GetMyGroups()
        {
            var response = await _groupService.GetGroupsByCreaterId(UserToken.Value.Id);
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
