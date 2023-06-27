using FriendBook.GroupService.API.BLL.gRPCClients.AccountService;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;
using FriendBook.GroupService.API.Domain.UserToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Newtonsoft.Json;

namespace FriendBook.GroupService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountStatusGroupController : ODataController
    {
        private readonly IAccountStatusGroupService _accountStatusGroupService;
        private readonly IValidationService<AccountStatusGroupDTO> _accountStatusGroupDTOValidationService;
        private readonly IGrpcService _grpcService;
        public Lazy<UserTokenAuth> UserToken { get; set; }
        public AccountStatusGroupController(IAccountStatusGroupService accountStatusGroupService, IValidationService<AccountStatusGroupDTO> validationService,
            IGrpcService grpcService, IHttpContextAccessor httpContextAccessor)
        {
            _accountStatusGroupService = accountStatusGroupService;
            _accountStatusGroupDTOValidationService = validationService;
            UserToken = new Lazy<UserTokenAuth>(() => UserTokenAuth.CreateUserToken(httpContextAccessor.HttpContext.User.Claims));
            _grpcService = grpcService;
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteAccountStatusGroup([FromQuery] Guid idGroupDeleted, [FromQuery] Guid idUserGuid)
        {
            var response = await _accountStatusGroupService.DeleteAccountStatusGroup(idUserGuid,UserToken.Value.Id, idGroupDeleted);
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateAccountStatusGroup([FromBody] AccountStatusGroupDTO accountStatusGroupDTO)
        {
            var responseValidation = await _accountStatusGroupDTOValidationService.ValidateAsync(accountStatusGroupDTO);
            if (responseValidation is not null)
                return Ok(responseValidation);

            BaseResponse<ResponseUserExists> responseAnotherAPI = await _grpcService.CheckUserExists(accountStatusGroupDTO.AccountId);

            if (!responseAnotherAPI.Data.Exists) 
            {
                return Ok(responseAnotherAPI);
            }

            var response = await _accountStatusGroupService.CreateAccountStatusGroup(accountStatusGroupDTO);
            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAccountStatusGroup([FromBody] AccountStatusGroupDTO accountStatusGroupDTO)
        {
            var responseValidation = await _accountStatusGroupDTOValidationService.ValidateAsync(accountStatusGroupDTO);
            if (responseValidation is not null)
                return Ok(responseValidation);

            var response = await _accountStatusGroupService.UpdateAccountStatusGroup(accountStatusGroupDTO, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpGet("GetProfilesByIdGroup")]
        public async Task<IActionResult> GetProfilesByIdGroup([FromQuery] Guid idGroup, [FromQuery] string login = "")
        {
            var responseAnotherApi = await _grpcService.GetProfiles(login, Request.Headers["Authorization"]);
            if (responseAnotherApi.Message is not null)
            {
                return Ok(responseAnotherApi);
            }

            var response = await _accountStatusGroupService.GetProfilesByIdGroup(idGroup, responseAnotherApi.Data);
            return Ok(response);
        }
    }
}
