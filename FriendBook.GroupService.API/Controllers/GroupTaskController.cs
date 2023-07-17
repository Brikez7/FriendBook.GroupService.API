﻿using FriendBook.GroupService.API.BLL.GrpcServices;
using FriendBook.GroupService.API.BLL.Helpers;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.GroupTaskDTOs;
using FriendBook.GroupService.API.Domain.Entities.Postgres;
using FriendBook.GroupService.API.Domain.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FriendBook.GroupService.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class GroupTaskController : ODataController
    {
        private readonly IGroupTaskService _groupTaskService;
        private readonly IAccountStatusGroupService _accountStatusGroupService;
        private readonly IValidationService<RequestGroupTaskNew> _groupTaskNewValidationService;
        private readonly IValidationService<RequestGroupTaskKey> _groupTaskKeyValidationService;
        private readonly IValidationService<RequestGroupTaskChanged> _groupTaskChangedValidationService;
        private readonly IGrpcService _grpcService;
        public Lazy<DataAccessToken> UserToken { get; set; }
        public GroupTaskController(IGroupTaskService groupTaskService, IAccountStatusGroupService accountStatusGroupService,
            IValidationService<RequestGroupTaskNew> requestGroupTaskNewValidationService, IValidationService<RequestGroupTaskKey> requestGroupTaskKeyValidationService,
            IValidationService<RequestGroupTaskChanged> requestGroupTaskChangedValidationService, IGrpcService grpcService, IHttpContextAccessor httpContextAccessor)
        {
            _groupTaskService = groupTaskService;
            _accountStatusGroupService = accountStatusGroupService;
            _groupTaskNewValidationService = requestGroupTaskNewValidationService;
            _groupTaskKeyValidationService = requestGroupTaskKeyValidationService;
            _groupTaskChangedValidationService = requestGroupTaskChangedValidationService;
            _grpcService = grpcService;
            UserToken = AccessTokenHelper.CreateUser(httpContextAccessor.HttpContext!.User.Claims);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteGroupTask([FromBody] RequestGroupTaskKey groupTaskKey)
        {
            var responseValidation = await _groupTaskKeyValidationService.ValidateAsync(groupTaskKey);
            if (responseValidation.StatusCode != Domain.Response.Code.EntityIsValidated)
                return Ok(responseValidation);

            var response = await _groupTaskService.DeleteGroupTask(groupTaskKey, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateGroupTask([FromBody] RequestGroupTaskNew newGroupTaskDTO)
        {
            var responseValidation = await _groupTaskNewValidationService.ValidateAsync(newGroupTaskDTO);
            if (responseValidation.StatusCode != Domain.Response.Code.EntityIsValidated)
                return Ok(responseValidation);

            var response = await _groupTaskService.CreateGroupTask(newGroupTaskDTO,UserToken.Value.Id, UserToken.Value.Login);
            return Ok(response);
        }


        [HttpPut("Update")]
        public async Task<IActionResult> UpdateGroupTask([FromBody] RequestGroupTaskChanged groupTaskDTO)
        {
            var responseValidation = await _groupTaskChangedValidationService.ValidateAsync(groupTaskDTO);
            if (responseValidation.StatusCode != Domain.Response.Code.EntityIsValidated)
                return Ok(responseValidation);

            var response = await _groupTaskService.UpdateGroupTask(groupTaskDTO, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpPut("SubscribeTask")]
        public async Task<IActionResult> SubscribeTask([FromBody] RequestGroupTaskKey groupTaskKeyDTO)
        {
            var responseValidation = await _groupTaskKeyValidationService.ValidateAsync(groupTaskKeyDTO);
            if (responseValidation.StatusCode != Domain.Response.Code.EntityIsValidated)
                return Ok(responseValidation);

            var response = await _groupTaskService.SubcsribeGroupTask(groupTaskKeyDTO,UserToken.Value.Id);
            return Ok(response);
        }
        [HttpPut("UnsubscribeTask")]
        public async Task<IActionResult> UnsubscribeTask([FromBody] RequestGroupTaskKey groupTaskKeyDTO)
        {
            var responseValidation = await _groupTaskKeyValidationService.ValidateAsync(groupTaskKeyDTO);
            if (responseValidation.StatusCode != Domain.Response.Code.EntityIsValidated)
                return Ok(responseValidation);

            var response = await _groupTaskService.UnsubcsribeGroupTask(groupTaskKeyDTO, UserToken.Value.Id);
            return Ok(response);
        }
        [HttpGet("OData/GetTasks/{groupId}")]
        [EnableQuery]
        public async Task<IActionResult> GetTasksByNameTaskAndGroupId([FromRoute] Guid groupId, [FromQuery] string nameTask = "")
        {
            var responseAccountStatusGroup = await _accountStatusGroupService.GetAccountStatusesGroupFromUserGroup(UserToken.Value.Id, groupId);
            if(responseAccountStatusGroup.StatusCode != Domain.Response.Code.AccountStatusGroupRead) 
                return Ok(responseAccountStatusGroup);

            var usersIdFromGroup = responseAccountStatusGroup.Data.Group.AccountStatusGroups.Select(x => x.AccountId).ToArray();
            var tasksFromGroup = responseAccountStatusGroup.Data.Group.GroupTasks.Where(x => x.Name.ToLower().Contains(nameTask.ToLower())).ToList();
            var isAdmin = responseAccountStatusGroup.Data.RoleAccount > RoleAccount.Default;

            var responseAnotherApi = await _grpcService.GetUsersLoginWithId(usersIdFromGroup);
            if (responseAnotherApi.StatusCode != Domain.Response.Code.GrpcUsersRead) 
                return Ok(responseAnotherApi);

            var response = _accountStatusGroupService.GetTasksPage(tasksFromGroup, responseAnotherApi.Data.Users.ToArray(), isAdmin);
            return Ok(response);
        }
    }
}
