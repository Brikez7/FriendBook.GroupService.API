using FriendBook.GroupService.API.BLL.gRPCClients.AccountClient;
using FriendBook.GroupService.API.BLL.GrpcServices;
using FriendBook.GroupService.API.BLL.Helpers;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FriendBook.GroupService.API.Controllers
{
    [Route("GroupService/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountStatusGroupController : ODataController
    {
        private readonly IAccountStatusGroupService _accountStatusGroupService;
        private readonly IValidationService<AccountStatusGroupDTO> _accountStatusGroupDTOValidationService;
        private readonly IGrpcClient _grpcService;
        public Lazy<DataAccessToken> UserToken { get; set; }
        public AccountStatusGroupController(IAccountStatusGroupService accountStatusGroupService, IValidationService<AccountStatusGroupDTO> validationService,
            IGrpcClient grpcService, IHttpContextAccessor httpContextAccessor)
        {
            _accountStatusGroupService = accountStatusGroupService;
            _accountStatusGroupDTOValidationService = validationService;
            UserToken = AccessTokenHelper.CreateUser(httpContextAccessor.HttpContext!.User.Claims);
            _grpcService = grpcService;
        }

        [HttpDelete("Delete/{groupId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid groupId, [FromQuery] Guid userId)
        {
            var response = await _accountStatusGroupService.DeleteAccountStatusGroup(userId,UserToken.Value.Id, groupId);
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] AccountStatusGroupDTO accountStatusGroupDTO)
        {
            var responseValidation = await _accountStatusGroupDTOValidationService.ValidateAsync(accountStatusGroupDTO);
            if (responseValidation.StatusCode != ServiceCode.EntityIsValidated)
                return Ok(responseValidation);

            BaseResponse<ResponseUserExists> responseAnotherAPI = await _grpcService.CheckUserExists(accountStatusGroupDTO.AccountId);

            if (responseAnotherAPI.StatusCode != ServiceCode.UserExists) 
            {
                return Ok(responseAnotherAPI);
            }

            var response = await _accountStatusGroupService.CreateAccountStatusGroup(UserToken.Value.Id,accountStatusGroupDTO);
            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] AccountStatusGroupDTO accountStatusGroupDTO)
        {
            var responseValidation = await _accountStatusGroupDTOValidationService.ValidateAsync(accountStatusGroupDTO);
            if (responseValidation.StatusCode != ServiceCode.EntityIsValidated)
                return Ok(responseValidation);

            var response = await _accountStatusGroupService.UpdateAccountStatusGroup(accountStatusGroupDTO, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpGet("GetProfilesByIdGroup")]
        public async Task<IActionResult> GetProfilesByGroupId([FromQuery] Guid groupId, [FromQuery] string login = "")
        {
            string accessToken = Request.Headers["Authorization"].ToString();

            var responseAnotherApi = await _grpcService.GetProfiles(login, accessToken);
            if (responseAnotherApi.StatusCode != ServiceCode.GrpcProfileReadied)
            {
                return Ok(responseAnotherApi);
            }

            var response = await _accountStatusGroupService.GetProfilesByIdGroup(groupId, responseAnotherApi.Data);
            return Ok(response);
        }
    }
}
