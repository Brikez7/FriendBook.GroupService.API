using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain;
using FriendBook.GroupService.API.Domain.CustomClaims;
using FriendBook.GroupService.API.Domain.DTO;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;
using FriendBook.GroupService.API.Domain.Requests;
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

        public AccountStatusGroupController(IAccountStatusGroupService accountStatusGroupService)
        {
            _accountStatusGroupService = accountStatusGroupService;
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteAccountStatusGroup([FromQuery] Guid idGroupDeleted, [FromQuery] Guid idUserGuid)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid createrId))
            {
                var response = await _accountStatusGroupService.DeleteAccountStatusGroup(idUserGuid,createrId, idGroupDeleted);
                return Ok(response);
            }

            return Ok(new StandartResponse<bool>
            {
                Message = "Not valid token",
                StatusCode = Domain.StatusCode.InternalServerError
            });
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateAccountStatusGroup([FromBody] AccountStatusGroupDTO accountStatusGroupDTO)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
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

                if (accountStatusGroupDTO.RoleAccount == RoleAccount.Creater) // validation
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
            return Ok(new StandartResponse<AccountStatusGroup>()
            { 
                Message = "Id not Guid",
                StatusCode = Domain.StatusCode.InternalServerError 
            });
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAccountStatusGroup([FromBody] AccountStatusGroupDTO accountStatusGroupDTO)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                var response = await _accountStatusGroupService.UpdateAccountStatusGroup(accountStatusGroupDTO, userId);
                return Ok(response);
            }
            return Ok(new StandartResponse<AccountStatusGroup>
            { 
                StatusCode = Domain.StatusCode.IdNotFound, 
                Message = "Id not found or user not outorisation" 
            });
        }

        [HttpGet("GetProfilesByIdGroup")]
        public async Task<IActionResult> GetProfilesByIdGroup([FromQuery] Guid idGroup, [FromQuery] string login = "")
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                StandartResponse<ProfileDTO[]> responseAnotherAPI;// New service
                try
                {
                    var reg_Req = new MyRequest($"https://localhost:7227/api/Contact/GetProfiles/{login}?", Request.Headers["Authorization"],null);
                    await reg_Req.SendRequest(MyTypeRequest.GET);
                    responseAnotherAPI = JsonConvert.DeserializeObject<StandartResponse<ProfileDTO[]>>(reg_Req.Response);
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
                return Ok(new StandartResponse<AccountStatusGroupDTO>()
                {
                    Message = $"Identity server: {responseAnotherAPI.Message}",
                    StatusCode = Domain.StatusCode.InternalServerError,
                });

            }
            return Ok(new StandartResponse<ProfileDTO[]>()
            {
                Message = "Token not valid",
                StatusCode = Domain.StatusCode.InternalServerError
            });
        }
    }
}
