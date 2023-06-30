using FriendBook.GroupService.API.BLL.gRPCClients.AccountService;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.Response;
using FriendBook.GroupService.API.Domain.UserToken;
using FriendBook.IdentityServer.API.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
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
        public Lazy<TokenAuth> UserToken { get; set; }
        public AccountStatusGroupController(IAccountStatusGroupService accountStatusGroupService, IValidationService<AccountStatusGroupDTO> validationService,
            IGrpcService grpcService, IHttpContextAccessor httpContextAccessor, IAccessTokenService accessTokenService)
        {
            _accountStatusGroupService = accountStatusGroupService;
            _accountStatusGroupDTOValidationService = validationService;
            UserToken = accessTokenService.CreateUser(httpContextAccessor.HttpContext!.User.Claims);
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
            if (responseValidation.StatusCode != Domain.Response.StatusCode.EntityIsValid)
                return Ok(responseValidation);

            BaseResponse<ResponseUserExists> responseAnotherAPI = await _grpcService.CheckUserExists(accountStatusGroupDTO.AccountId);

            if (responseAnotherAPI.StatusCode != Domain.Response.StatusCode.UserExists) 
            {
                return Ok(responseAnotherAPI);
            }

            var response = await _accountStatusGroupService.CreateAccountStatusGroup(UserToken.Value.Id,accountStatusGroupDTO);
            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAccountStatusGroup([FromBody] AccountStatusGroupDTO accountStatusGroupDTO)
        {
            var responseValidation = await _accountStatusGroupDTOValidationService.ValidateAsync(accountStatusGroupDTO);
            if (responseValidation.StatusCode != Domain.Response.StatusCode.EntityIsValid)
                return Ok(responseValidation);

            var response = await _accountStatusGroupService.UpdateAccountStatusGroup(accountStatusGroupDTO, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpGet("GetProfilesByIdGroup")]
        public async Task<IActionResult> GetProfilesByIdGroup([FromQuery] Guid idGroup, [FromQuery] string login = "")
        {
            string accessToken = (Request?.Headers["Authorization"] ?? "").ToString();

            var responseAnotherApi = await _grpcService.GetProfiles(login, accessToken);
            if (responseAnotherApi.StatusCode != Domain.Response.StatusCode.GrpcProphileRead)
            {
                return Ok(responseAnotherApi);
            }

            var response = await _accountStatusGroupService.GetProfilesByIdGroup(idGroup, responseAnotherApi.Data);
            return Ok(response);
        }
    }
}
