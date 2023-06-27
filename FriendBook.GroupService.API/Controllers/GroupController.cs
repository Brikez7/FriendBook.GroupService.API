using FriendBook.GroupService.API.BLL.gRPCClients.AccountService;
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
        private readonly IContactGroupService _groupService;
        private readonly IValidationService<GroupDTO> _groupDTOValidationService;
        private readonly IGrpcService _grpcService;
        public Lazy<UserTokenAuth> UserToken { get; set; }
        public GroupController(IContactGroupService groupService, IValidationService<GroupDTO> validationService, IGrpcService grpcService)
        {
            _groupService = groupService;
            _groupDTOValidationService = validationService;
            UserToken = new Lazy<UserTokenAuth>(() => UserTokenAuth.CreateUserToken(User.Claims));
            _grpcService = grpcService;
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
            BaseResponse<ResponseUserExists> responseAnotherAPI = await _grpcService.CheckUserExists(UserToken.Value.Id);

            if (!responseAnotherAPI.Data.Exists)
            {
                return Ok(responseAnotherAPI);
            }

            var response = await _groupService.CreateGroup(groupName, UserToken.Value.Id);
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
