﻿using FriendBook.GroupService.API.BLL.gRPCClients.AccountService;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.GroupDTOs;
using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.API.Domain.UserToken;
using FriendBook.IdentityServer.API.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using StatusCode = FriendBook.GroupService.API.Domain.Response.StatusCode;

namespace FriendBook.GroupService.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class GroupController : ODataController
    {
        private readonly IContactGroupService _groupService;
        private readonly IValidationService<GroupDTO> _groupDTOValidationService;
        private readonly IGrpcService _grpcService;
        public Lazy<TokenAuth> UserToken { get; set; }
        public GroupController(IContactGroupService groupService, IValidationService<GroupDTO> validationService, IGrpcService grpcService, IAccessTokenService accessTokenService, IHttpContextAccessor httpContext)
        {
            _groupService = groupService;
            _groupDTOValidationService = validationService;
            UserToken = accessTokenService.CreateUser(httpContext.HttpContext!.User.Claims);
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
         
            if (responseAnotherAPI.StatusCode != Domain.Response.StatusCode.UserExists)
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
            if (responseValidation.StatusCode == Domain.Response.StatusCode.ErrorValidation)
                return Ok(responseValidation);

            var response = await _groupService.UpdateGroup(groupDTO, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpGet("OData/GetMyGroups")]
        [EnableQuery]
        public async Task<IActionResult> GetMyGroups()
        {
            var response = await _groupService.GetGroupsByUserId(UserToken.Value.Id);
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
