using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain;
using FriendBook.GroupService.API.Domain.CustomClaims;
using FriendBook.GroupService.API.Domain.DTO;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;
using FriendBook.GroupService.API.Domain.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;

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
        public async Task<IActionResult> DeleteAccountStatusGroup([FromQuery] string idGroup, [FromQuery] string idUser)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid createrId))
            {
                Guid idGroupDeleted;
                if (!Guid.TryParse(idGroup, out idGroupDeleted))
                {
                    return Ok(new StandartResponse<ProfileDTO[]>()
                    {
                        Message = "Id group not valid",
                        StatusCode = Domain.StatusCode.InternalServerError
                    });
                }
                Guid idUserGuid;
                if (!Guid.TryParse(idUser, out idUserGuid))
                {
                    return Ok(new StandartResponse<ProfileDTO[]>()
                    {
                        Message = "Id user not valid",
                        StatusCode = Domain.StatusCode.InternalServerError
                    });
                }

                var response = await _accountStatusGroupService.DeleteAccountStatusGroup(idUserGuid,createrId, idGroupDeleted);

                return Ok(new StandartResponse<bool>
                {
                    Data = response.Data
                });
            }

            return Ok(new StandartResponse<bool>
            {
                Message = "Not valid token",
                StatusCode = Domain.StatusCode.InternalServerError
            });
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateAccountStatusGroup(AccountStatusGroupDTO accountStatusGroupDTO)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                BaseResponse<bool> responseAnotherAPI;
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
                    var newAccountStatusGroup = new AccountStatusGroup(accountStatusGroupDTO);
                    var response = await _accountStatusGroupService.CreateAccountStatusGroup(newAccountStatusGroup);

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
        public async Task<IActionResult> UpdateAccountStatusGroup(AccountStatusGroupDTO accountStatusGroupDTO)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                var changingStatusGroup = new AccountStatusGroup(accountStatusGroupDTO);
                var response = await _accountStatusGroupService.UpdateAccountStatusGroup(changingStatusGroup, userId);

                return Ok(response);
            }
            return Ok(new StandartResponse<AccountStatusGroup>
            { 
                StatusCode = Domain.StatusCode.IdNotFound, 
                Message = "Id not found or user not outorisation" 
            });
        }

        [HttpGet("OData/Groups")]
        [EnableQuery]
        public async Task<IActionResult> GetAccountStatusGroups(string idGroup)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                Guid groupId;
                if (!Guid.TryParse(idGroup, out groupId))
                {
                    return Ok(new StandartResponse<ProfileDTO[]>()
                    {
                        Message = "Id group not valid",
                        StatusCode = Domain.StatusCode.InternalServerError
                    });
                }

                var accountsStatusGroups = await _accountStatusGroupService.GetAccountStatusGroupOData().Data
                                           .Where(x => x.IdGroup == groupId)
                                           .Select(x => new AccountStatusGroupDTO((Guid)x.IdGroup, x.AccountId, x.RoleAccount))
                                           .ToArrayAsync();

                if (accountsStatusGroups == null)
                {
                    return Ok(new StandartResponse<AccountStatusGroupDTO[]>
                    {
                        StatusCode = Domain.StatusCode.IdNotFound,
                        Message = "Id not found or user not outorisation"
                    });
                }

                return Ok(new StandartResponse<AccountStatusGroupDTO[]>
                {
                    Data = accountsStatusGroups
                });
            }
            return Ok(new StandartResponse<AccountStatusGroupDTO[]>()
            {
                Message = "User not authorization or nekorrect token",
                StatusCode = Domain.StatusCode.InternalServerError
            });
        }

        [HttpGet("GetProfilesByIdGroup")]
        public async Task<IActionResult> GetProfilesByIdGroup([FromQuery] string idGroup, [FromQuery] string login = "")
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                if (!Guid.TryParse(idGroup, out Guid idGroupGuid)) 
                {
                    return Ok(new StandartResponse<ProfileDTO[]>()
                    {
                        Message = "Id group not valid",
                        StatusCode = Domain.StatusCode.InternalServerError
                    });
                }

                StandartResponse<ProfileDTO[]> responseAnotherAPI;
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
                    var usersInSearchedGroudId = await _accountStatusGroupService.GetAccountStatusGroupOData().Data.Where(x => x.IdGroup == idGroupGuid).Select(x =>  x.AccountId ).ToArrayAsync();

                    if (usersInSearchedGroudId is null || usersInSearchedGroudId.Length == 0) 
                    {
                        return Ok(new StandartResponse<AccountStatusGroupDTO>()
                        {
                            Message = "Group not found",
                            StatusCode = Domain.StatusCode.InternalServerError,
                        });
                    }

                    var usersInGroup = responseAnotherAPI.Data.Join(usersInSearchedGroudId,
                                       profile => profile.Id,
                                       id => id,
                                       (profile, id) => profile);

                    return Ok(new StandartResponse<ProfileDTO[]>{
                        Data = usersInGroup.ToArray(),
                    });;
                }
                return Ok(new StandartResponse<AccountStatusGroupDTO>()
                {
                    Message = "Identity server not connected",
                    StatusCode = Domain.StatusCode.InternalServerError,
                });

            }
            return Ok(new StandartResponse<ProfileDTO[]>()
            {
                Message = "User not authorization",
                StatusCode = Domain.StatusCode.InternalServerError
            });
        }
    }
}
