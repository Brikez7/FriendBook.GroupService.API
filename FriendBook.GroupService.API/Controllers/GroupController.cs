using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.CustomClaims;
using FriendBook.GroupService.API.Domain.DTO;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.GroupService.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class GroupController : ODataController
    {
        private readonly IGroupService _groupService;
        private readonly IAccountStatusGroupService _accountStatusGroupService;
        public GroupController(IGroupService groupService, IAccountStatusGroupService accountStatusGroupService)
        {
            _accountStatusGroupService = accountStatusGroupService;
            _groupService = groupService;
        }

        [HttpDelete("Delete/{idGroup}")]
        public async Task<IActionResult> DeleteGroup(string idGroup)
        {
            string? id = User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value;
            var idUser = Guid.Parse(id);
            var idGroupGuid = Guid.Parse(idGroup);
            /// Get group
            var group = await _groupService.GetGroupOData().Data
                ?.Where(x => x.Id == idGroupGuid)
                .AsNoTracking()
                .SingleOrDefaultAsync();
                if (group == null)
                {
                    return Ok(new StandartResponse<string>
                    {
                        Message = "group not found",
                        StatusCode = Domain.StatusCode.IdNotFound
                    });
                }
                else if (group.CreaterId == idUser)
                {
                    var response = await _groupService.DeleteGroup(idGroupGuid);

                    return Ok(response);
                }

                return Ok(new StandartResponse<string> 
                {
                    Message = "group not found",
                    StatusCode = Domain.StatusCode.IdNotFound
                });
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateGroup([FromBody] string groupName)
        {
            string? id = User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value;
            var userId = Guid.Parse(id);

            var newGroup = new Group(groupName, userId);
            var response = await _groupService.CreateGroup(newGroup);

            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<BaseResponse<GroupDTO>> UpdateGroup([FromBody] GroupDTO groupDTO)
        {
            string? id = User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value;
            var userId = Guid.Parse(id);

            var newGroup = new Group(groupDTO, userId);

            var response = await _groupService.UpdateGroup(newGroup);

            return response;
        }

        [HttpGet("OData/GetMyGroups")]
        [EnableQuery]
        public async Task<BaseResponse<GroupDTO[]>> GetMyGroups()
        {
            string? id = User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value;
            var userId = Guid.Parse(id);
            var listGroupDTO = (await _groupService.GetGroupOData().Data.Where(x => x.CreaterId == userId).Select(x => new GroupDTO(x)).ToArrayAsync()) ?? new GroupDTO[] { };

            return new StandartResponse<GroupDTO[]>() { Data = listGroupDTO };
        }
    }
}
