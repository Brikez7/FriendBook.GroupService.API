using FriendBook.GroupService.API.BLL.gRPCClients.AccountClient;
using FriendBook.GroupService.API.BLL.GrpcServices;
using FriendBook.GroupService.API.BLL.Helpers;
using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.JWT;
using FriendBook.GroupService.API.Domain.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendBook.GroupService.API.Controllers
{
    [Route("GroupService/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountStatusGroupController : ControllerBase
    {
        private readonly IAccountStatusGroupService _accountStatusGroupService;
        private readonly IValidationService<RequestNewAccountStatusGroup> _requestNewValidationService;
        private readonly IValidationService<RequestUpdateAccountStatusGroup> _requestUpdateValidationService;
        private readonly IGrpcClient _grpcService;
        public Lazy<DataAccessToken> UserToken { get; set; }
        public AccountStatusGroupController(IAccountStatusGroupService accountStatusGroupService, IValidationService<RequestNewAccountStatusGroup> validationService1,
            IValidationService<RequestUpdateAccountStatusGroup> validationService, IGrpcClient grpcService, IHttpContextAccessor httpContextAccessor)
        {
            _accountStatusGroupService = accountStatusGroupService;
            _requestUpdateValidationService = validationService;
            _requestNewValidationService = validationService1;
            UserToken = AccessTokenHelper.CreateUser(httpContextAccessor.HttpContext!.User.Claims);
            _grpcService = grpcService;
        }

        [HttpDelete("Delete/{accountStatusGroupId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid accountStatusGroupId)
        {
            var response = await _accountStatusGroupService.DeleteAccountStatusGroup(accountStatusGroupId, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] RequestNewAccountStatusGroup requestNewAccountStatusGroup)
        {
            var responseValidation = await _requestNewValidationService.ValidateAsync(requestNewAccountStatusGroup);
            if (responseValidation.ServiceCode != ServiceCode.EntityIsValidated)
                return Ok(responseValidation);

            BaseResponse<ResponseUserExists> responseAnotherAPI = await _grpcService.CheckUserExists(requestNewAccountStatusGroup.AccountId);
            if (responseAnotherAPI.ServiceCode != ServiceCode.UserExists) 
                return Ok(responseAnotherAPI);

            var response = await _accountStatusGroupService.CreateAccountStatusGroup(UserToken.Value.Id,requestNewAccountStatusGroup);
            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] RequestUpdateAccountStatusGroup requestUpdateAccountStatusGroup)
        {
            var responseValidation = await _requestUpdateValidationService.ValidateAsync(requestUpdateAccountStatusGroup);
            if (responseValidation.ServiceCode != ServiceCode.EntityIsValidated)
                return Ok(responseValidation);

            var response = await _accountStatusGroupService.UpdateAccountStatusGroup(requestUpdateAccountStatusGroup, UserToken.Value.Id);
            return Ok(response);
        }

        [HttpGet("GetProfilesByIdGroup")]
        public async Task<IActionResult> GetProfilesByGroupId([FromQuery] Guid groupId, [FromQuery] string login = "")
        {
            var responseAnotherApi = await _grpcService.GetProfiles(login, Request.Headers["Authorization"].ToString());
            if (responseAnotherApi.ServiceCode != ServiceCode.GrpcProfileReadied)
                return Ok(responseAnotherApi);

            var response = await _accountStatusGroupService.GetProfilesByIdGroup(groupId, responseAnotherApi.Data);
            return Ok(response);
        }
    }
}
