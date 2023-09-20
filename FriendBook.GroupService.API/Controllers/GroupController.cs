using FriendBook.GroupService.API.BLL.gRPCClients.AccountClient;
using FriendBook.GroupService.API.BLL.GrpcServices;
using FriendBook.GroupService.API.BLL.Helpers;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendBook.GroupService.API.Controllers
{
    [ApiController]
    [Route("GroupService/v1/[controller]")]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IValidationService<RequestUpdateGroup> _groupDTOValidationService;
        private readonly IGrpcClient _grpcIdentityClient;
        public Lazy<AccessToken> UserToken { get; set; }
        public GroupController(IGroupService groupService, IValidationService<RequestUpdateGroup> validationService, IGrpcClient grpcService, IHttpContextAccessor httpContext)
        {
            _groupService = groupService;
            _groupDTOValidationService = validationService;
            UserToken = AccessTokenHelper.CreateUser(httpContext.HttpContext!.User.Claims);
            _grpcIdentityClient = grpcService;
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
            BaseResponse<ResponseUserExists> responseAnotherAPI = await _grpcIdentityClient.CheckUserExists(UserToken.Value.Id);
            if (responseAnotherAPI.ServiceCode != ServiceCode.UserExists)
                return Ok(responseAnotherAPI);

            var response = await _groupService.CreateGroup(groupName, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateGroup([FromBody] RequestUpdateGroup requestUpdateGroup)
        {
            var responseValidation = await _groupDTOValidationService.ValidateAsync(requestUpdateGroup);
            if (responseValidation.ServiceCode == ServiceCode.EntityIsNotValidated)
                return Ok(responseValidation);

            var response = await _groupService.UpdateGroup(requestUpdateGroup, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpGet("GetMyGroups")]
        public async Task<IActionResult> GetMyGroups()
        {
            var response = await _groupService.GetGroupsByCreatorId(UserToken.Value.Id);
            return Ok(response);
        }

        [HttpGet("GetMyGroupsWithMyStatus")]
        public async Task<IActionResult> GetMyGroupsWithMyStatus()
        {
            var response = await _groupService.GetGroupsWithStatusByUserId(UserToken.Value.Id);
            return Ok(response);
        }
    }
}
