using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.CustomClaims;
using FriendBook.GroupService.API.Domain.DTO;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.GroupService.API.Controllers
{
    [Route("api/Group[controller]")]
    public class GroupController : Controller
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpDelete("Group")]
        public async Task<BaseResponse<IActionResult>> DeleteGroup([FromQuery] Guid id)
        {
            Guid userId;
            if (Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value, out userId))
            {
                var group = await _groupService.GetGroupOData().Data
                ?.Where(x => x.Id == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
                if (group == null)
                {
                    return new StandartResponse<IActionResult>
                    {
                        Message = "group not found",
                        Data = NotFound()
                    };
                }
                else if (group.CreatedId == userId)
                {
                    var resourse = await _groupService.DeleteGroup(id);

                    return new StandartResponse<IActionResult>
                    {
                        Data = NoContent()
                    };
                }

                return  new StandartResponse<IActionResult> 
                { 
                    Data = Forbid()
                };
            }

            return new StandartResponse<IActionResult> { 
                Data = StatusCode(((int)Domain.StatusCode.IdNotFound)) 
            };
        }
        [HttpPost("Create")]
        public async Task<BaseResponse<Group>> CreateGroup(GroupDTO groupDTO)
        {
            Guid userId;
            if (Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value, out userId))
            {
                var newGroup = new Group(groupDTO,userId);
                var resourse = await _groupService.CreateGroup(newGroup);

                return resourse;
            }
            return new StandartResponse<Group> 
            { 
                StatusCode = Domain.StatusCode.IdNotFound,
                Message = "Id not found or user not фutorisation"
            };
        }
        [HttpPost("Group")]
        public async Task<BaseResponse<Group>> UpdateGroup(GroupDTO groupDTO)
        {
            Guid userId;
            if (Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value, out userId))
            {
                var newGroup = new Group(groupDTO, userId);
                var resourse = await _groupService.UpdateGroup(newGroup);

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
