using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.CustomClaims;
using FriendBook.GroupService.API.Domain.DTO;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FriendBook.GroupService.API.Controllers
{
    public class GroupController : Controller
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService commentService)
        {
            _groupService = commentService;
        }
        /// <summary>
        /// Созвониться с Ильей
        /// </summary>
        [Authorize]
        [HttpDelete("Group")]
        public async Task<IActionResult> DeleteGroup([FromQuery] Guid ownId, [FromQuery] Guid id)
        {
            var identity = (HttpContext.User.Identity as ClaimsIdentity).Claims;
            string? guid = identity.First(x => x.Type == CustomClaimType.AccountId).Value;

            Guid userId;
            if (Guid.TryParse(guid, out userId))
            {
                var comment = await _groupService.GetGroupOData().Data
                ?.Where(x => x.Id == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
                if (comment == null)
                {

                    return NotFound();
                }
                else if (comment.AccountId == ownId)
                {
                    var resourse = await _groupService.DeleteGroup(id);

                    return NoContent();
                }

                return Forbid();
            }

            return StatusCode(((int)Domain.StatusCode.IdNotFound));
        }
        [Authorize]
        [HttpPost("CreateGroup")]
        public async Task<BaseResponse<Group>> CreateGroup(GroupDTO groupDTO)
        {
            var identity = (HttpContext.User.Identity as ClaimsIdentity).Claims;
            string? guid = identity.First(x => x.Type == CustomClaimType.AccountId).Value;
            Guid userId;
            if (Guid.TryParse(guid, out userId))
            {
                var newGroup = new Group(groupDTO,userId);
                var resourse = await _groupService.CreateGroup(newGroup);

                return resourse;
            }
            return new StandartResponse<Group> 
            { 
                StatusCode = Domain.StatusCode.IdNotFound,
                Message = "Id not found or user not outorisation"
            };
        }
    }
}
