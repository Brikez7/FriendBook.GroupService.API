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
    public class GroupTaskController : ODataController
    {
        private readonly IGroupTaskService _groupTaskService;

        public GroupTaskController(IGroupTaskService groupTaskService)
        {
            _groupTaskService = groupTaskService;
        }

        [HttpDelete("Delete")]
        public async Task<BaseResponse<IActionResult>> DeleteGroupTask([FromQuery] Guid id)
        {
            Guid userId;
            if (Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value, out userId))
            {
                var groupTask = await _groupTaskService.GetGroupTaskOData().Data
                ?.Where(x => x.Id == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
                if (groupTask == null)
                {
                    return new StandartResponse<IActionResult>
                    {
                        Message = "group not found",
                        Data = NotFound()
                    };
                }
                else if (groupTask.AccountId == userId)
                {
                    var resourse = await _groupTaskService.DeleteGroupTask(id);

                    return new StandartResponse<IActionResult>
                    {
                        Data = NoContent()
                    };
                }

                return new StandartResponse<IActionResult>
                {
                    Data = Forbid()
                };
            }

            return new StandartResponse<IActionResult>
            {
                Data = StatusCode(((int)Domain.StatusCode.IdNotFound))
            };
        }
        [HttpPost("Create")]
        public async Task<BaseResponse<GroupTask>> CreateGroup(GroupTaskDTO groupDTO)
        {
            Guid userId;
            if (Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value, out userId))
            {
                /// Испроавить
                var newGroupTask = new GroupTask(groupDTO, userId);
                var resourse = await _groupTaskService.CreateGroupTask(newGroupTask);

                return resourse;
            }
            return new StandartResponse<GroupTask>
            {
                StatusCode = Domain.StatusCode.IdNotFound,
                Message = "Id not found or user not autorisation"
            };
        }
        [HttpPut("Update")]
        public async Task<BaseResponse<GroupTask>> UpdateGroup(GroupTaskDTO groupDTO)
        {
            Guid userId;
            if (Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value, out userId))
            {
                /// исправить
                var newGroupTask = new GroupTask(groupDTO, userId);
                var resourse = await _groupTaskService.UpdateGroupTask(newGroupTask);

                return resourse;
            }
            return new StandartResponse<GroupTask>
            {
                StatusCode = Domain.StatusCode.IdNotFound,
                Message = "Id not found or user not outorisation"
            };
        }
        [HttpGet("OData/Get")]
        [EnableQuery]
        public IQueryable<GroupTask> GetGroups()
        {
            return _groupTaskService.GetGroupTaskOData().Data;
        }
    }
}
