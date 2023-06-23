using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.DTO.AccountStatusGroupDTOs;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;
using FriendBook.GroupService.API.Domain.Requests;
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
        public Lazy<UserTokenAuth> UserToken { get; set; }
        public AccountStatusGroupController(IAccountStatusGroupService accountStatusGroupService, IValidationService<AccountStatusGroupDTO> validationService)
        {
            _accountStatusGroupService = accountStatusGroupService;
            _accountStatusGroupDTOValidationService = validationService;
            UserToken = new Lazy<UserTokenAuth>(() => UserTokenAuth.CreateUserToken(User.Claims));
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

            BaseResponse<bool> responseAnotherAPI; // New service
            try 
            {
                var reg_Req = new MyRequest($"https://localhost:7227/api/IdentityServer/checkUserExists?userId={accountStatusGroupDTO.AccountId}", null,null);
                await reg_Req.SendRequest(MyTypeRequest.GET);
                responseAnotherAPI = JsonConvert.DeserializeObject<StandartResponse<bool>>(reg_Req.Response);
            }
            catch (Exception e)
            {
                return Ok(new StandartResponse<AccountStatusGroup>()
                {
                    Message = $"Identity server not responsing {e.Message}",
                    StatusCode = Domain.StatusCode.InternalServerError,
                });
            }

            if (accountStatusGroupDTO.RoleAccount == RoleAccount.Creater)
            {
                return Ok(new StandartResponse<AccountStatusGroupDTO>()
                {
                    Message = "Account not been status creator",
                    StatusCode = Domain.StatusCode.InternalServerError,
                });
            }
            if (responseAnotherAPI is not null && responseAnotherAPI.Data)
            {
                var response = await _accountStatusGroupService.CreateAccountStatusGroup(accountStatusGroupDTO);
                return Ok(response);
            }
            return Ok(new StandartResponse<AccountStatusGroupDTO>()
            {
                Message = "Account not exists or server not connected",
                StatusCode = Domain.StatusCode.InternalServerError,
            });
            
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
            StandartResponse<ResponseProfile[]> responseAnotherAPI;
            try
            {
                var reg_Req = new MyRequest($"https://localhost:7227/api/Contact/GetProfiles/{login}?", Request.Headers["Authorization"],null);
                await reg_Req.SendRequest(MyTypeRequest.GET);
                responseAnotherAPI = JsonConvert.DeserializeObject<StandartResponse<ResponseProfile[]>>(reg_Req.Response);
            }
            catch (Exception e)
            {
                return Ok(new StandartResponse<AccountStatusGroup>()
                {
                    Message = $"Identity server not responsing {e.Message}",
                    StatusCode = Domain.StatusCode.InternalServerError,
                });
            }

            if (responseAnotherAPI.Message is null && responseAnotherAPI.Data is not null)
            {
                var response = await _accountStatusGroupService.GetProfilesByIdGroup(idGroup, responseAnotherAPI.Data);
                return Ok(response);
            }

            
            return Ok(new StandartResponse<ResponseProfile[]>()
            {
                Message = "Token not valid",
                StatusCode = Domain.StatusCode.InternalServerError
            });
        }
    }
}
