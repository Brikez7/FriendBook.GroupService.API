using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.CustomClaims;
using FriendBook.GroupService.API.Domain.DTO;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.GroupService.API.Controllers
{
    [Route("api/AccountStatusGroup[controller]")]
    [ApiController]
    [Authorize]
    public class AccountStatusGroupController : ControllerBase
    {
        private readonly IAccountStatusGroupService _accountStatusGroupService;
        private readonly IGroupService _groupService;

        public AccountStatusGroupController(IAccountStatusGroupService accountStatusGroupService, IGroupService groupService)
        {
            _accountStatusGroupService = accountStatusGroupService;
            _groupService = groupService;
        }

        [HttpDelete("AccountStatusGroup")]
        public async Task<IActionResult> DeleteAccountStatusGroup([FromQuery] Guid id)
        {
            Guid userId;
            if (Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value, out userId))
            {
                var accountStatusGroup = await _accountStatusGroupService.GetAccountStatusGroupOData().Data
                ?.Where(x => x.Id == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
                if (accountStatusGroup == null)
                {
                    return NotFound();
                }
                else if (await _groupService.GetGroupOData().Data.AnyAsync(x=> x.CreatedId == userId))
                {
                    var resourse = await _accountStatusGroupService.DeleteAccountStatusGroup(id);

                    return NoContent();
                }

                return Forbid();
            }

            return StatusCode(((int)Domain.StatusCode.IdNotFound));
        }
        [HttpPost("AccountStatusGroup")]
        public async Task<BaseResponse<AccountStatusGroup>> CreateAccountStatusGroup(AccountStatusGroupDTO accountStatusGroupDTO)
        {
            Guid userId;
            if (Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value, out userId))
            {
                var newGroup = new AccountStatusGroup(accountStatusGroupDTO);
                var resourse = await _accountStatusGroupService.CreateAccountStatusGroup(newGroup);

                return resourse;
            }
            return new StandartResponse<AccountStatusGroup>
            {
                StatusCode = Domain.StatusCode.IdNotFound,
                Message = "Id not found or user not outorisation"
            };
        }
        [HttpPost("Group")]
        public async Task<BaseResponse<AccountStatusGroup>> UpdateAccountStatusGroup(AccountStatusGroupDTO accountStatusGroupDTO)
        {
            Guid userId;
            if (Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value , out userId))
            {
                var newGroup = new AccountStatusGroup(accountStatusGroupDTO);
                var resourse = await _accountStatusGroupService.UpdateAccountStatusGroup(newGroup);

                return resourse;
            }
            return new StandartResponse<AccountStatusGroup>
            {
                StatusCode = Domain.StatusCode.IdNotFound,
                Message = "Id not found or user not outorisation"
            };
        }
    }
}
