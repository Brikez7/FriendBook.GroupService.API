using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.CustomClaims;
using FriendBook.GroupService.API.Domain.DTO;
using FriendBook.GroupService.API.Domain.DTO.GroupTasksDTO;
using FriendBook.GroupService.API.Domain.Entities;
using FriendBook.GroupService.API.Domain.InnerResponse;
using FriendBook.GroupService.API.Domain.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FriendBook.GroupService.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class GroupTaskController : ODataController
    {
        private readonly IGroupTaskService _groupTaskService;
        private readonly IAccountStatusGroupService _accountStatusGroupService;
        public GroupTaskController(IGroupTaskService groupTaskService, IAccountStatusGroupService accountStatusGroupService)
        {
            _groupTaskService = groupTaskService;
            _accountStatusGroupService = accountStatusGroupService;
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteGroupTask([FromBody] GroupTaskKeyDTO groupTaskKey)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                var deletedGroupTask = new GroupTask(groupTaskKey);
                var response = await _groupTaskService.DeleteGroupTask(deletedGroupTask, userId);

                if (response is null)
                {
                    return Ok(new StandartResponse<bool>
                    {
                        Message = "The task was not deleted",
                        StatusCode = Domain.StatusCode.InternalServerError
                    });
                }
                return Ok(response);

            }
            return Ok(new StandartResponse<bool>
            {
                Message = "Not valid token",
                StatusCode = Domain.StatusCode.InternalServerError
            });
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateGroupTask([FromBody] GroupTaskNewDTO groupTaskDTO)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                var newGroupTask = new GroupTask(groupTaskDTO, userId);
                var response = await _groupTaskService.CreateGroupTask(newGroupTask);

                if (response.Data != null)
                {
                    return Ok(new StandartResponse<GroupTaskViewDTO>
                    {
                        Data = new GroupTaskViewDTO(response.Data, User.Claims.First(x => x.Type == CustomClaimType.Login).Value)
                    });
                }

                return Ok(response);
            }
            return Ok(new StandartResponse<GroupTaskViewDTO>
            {
                Message = "Not valid token",
                StatusCode = Domain.StatusCode.InternalServerError
            });
        }


        [HttpPut("Update")]
        public async Task<IActionResult> UpdateGroupTask([FromBody] GroupTaskChangedDTO groupTaskDTO)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                var newGroupTask = new GroupTask(groupTaskDTO);
                var response = await _groupTaskService.UpdateGroupTask(newGroupTask,groupTaskDTO.NewName,userId);

                if (response.Data != null)
                {
                    return Ok(new StandartResponse<GroupTaskViewDTO>
                    {
                        Data = new GroupTaskViewDTO(response.Data)
                    });
                }

                return Ok(response);
            }
            return Ok(new StandartResponse<GroupTaskViewDTO>
            {
                Message = "Not valid token",
                StatusCode = Domain.StatusCode.InternalServerError
            });
        }
        [HttpPut("SubscribeTask")]
        public async Task<IActionResult> SubscribeTask([FromBody] GroupTaskKeyDTO groupDTO)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                var newGroupTask = new GroupTask(groupDTO);
                var response = await _groupTaskService.SubcsribeGroupTask(newGroupTask,userId);

                return Ok(response);
            }
            return Ok(new StandartResponse<GroupTask>
            {
                StatusCode = Domain.StatusCode.IdNotFound,
                Message = "Id not found or user not autorization"
            });
        }
        [HttpPut("UnsubscribeTask")]
        public async Task<IActionResult> UnsubscribeTask([FromBody] GroupTaskKeyDTO groupDTO)
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                var newGroupTask = new GroupTask(groupDTO);
                var response = await _groupTaskService.UnsubcsribeGroupTask(newGroupTask, userId);

                return Ok(response);
            }
            return Ok(new StandartResponse<GroupTask>
            {
                StatusCode = Domain.StatusCode.IdNotFound,
                Message = "Id not found or user not autorization"
            });
        }
        [HttpGet("OData/GetTasks")]
        [EnableQuery]
        public async Task<IActionResult> GetTasksByNameTaskAndIdGroup([FromQuery] Guid idGroup, [FromQuery] string nameTask = "")
        {
            if (Guid.TryParse(User.Claims.First(x => x.Type == CustomClaimType.AccountId).Value, out Guid userId))
            {
                var accountStatusGroup = await _accountStatusGroupService.GetAccountStatusGroupOData().Data
                                                                         .Where(x => x.AccountId == userId)
                                                                         .Include(x => x.Group)
                                                                         .ThenInclude(x => x.GroupTasks)
                                                                         .Include(x => x.Group)
                                                                         .ThenInclude(x => x.AccountStatusGroups)
                                                                         .FirstOrDefaultAsync(x => x.Group != null && x.Group.Id == idGroup);

                if (accountStatusGroup == null)
                {
                    return Ok(new StandartResponse<TasksPageDTO>
                    {
                        Message = "Account not found",
                        StatusCode = Domain.StatusCode.InternalServerError
                    });
                }
                else if (accountStatusGroup.Group == null) 
                {
                    return Ok(new StandartResponse<TasksPageDTO>
                    {
                        Message = "Group not found",
                        StatusCode = Domain.StatusCode.InternalServerError
                    });
                }
                else if (accountStatusGroup.Group.GroupTasks is null)
                {
                    return Ok(new StandartResponse<TasksPageDTO>
                    {
                        Message = "Tasks no found",
                        StatusCode = Domain.StatusCode.InternalServerError
                    });
                }


                bool isAdmin = accountStatusGroup.RoleAccount > RoleAccount.Default;

                var usersId = accountStatusGroup.Group.AccountStatusGroups.Select(x => x.AccountId).ToArray();

                var jsonUsers = JsonConvert.SerializeObject(usersId);

                BaseResponse<Tuple<Guid, string>[]> responseAnotherAPI;
                try
                {
                    var reg_Req = new MyRequest($"https://localhost:7227/api/IdentityServer/getLoginsUsers",null, jsonUsers);
                    await reg_Req.SendRequest(MyTypeRequest.POST);
                    responseAnotherAPI = JsonConvert.DeserializeObject<StandartResponse<Tuple<Guid, string>[]>>(reg_Req.Response);
                }
                catch (Exception e)
                {
                    return Ok(new StandartResponse<AccountStatusGroup>()
                    {
                        Message = $"Identity server not responsing {e.Message}",
                        StatusCode = Domain.StatusCode.InternalServerError,
                    });
                }

                var tasks = accountStatusGroup.Group.GroupTasks.Where(x => x.Name.ToLower().Contains(nameTask.ToLower())).ToList();

                List<GroupTaskViewDTO> tasksPages = new List<GroupTaskViewDTO>();
                foreach (var task in tasks)
                {
                    var namesUser = task.Team.Join(
                                            responseAnotherAPI.Data,
                                            userId => userId,
                                            loginWithIdUser => loginWithIdUser.Item1,
                                            (task, loginWithIdUser) => loginWithIdUser.Item2).ToArray();

                    GroupTaskViewDTO groupTaskViewDTO = new GroupTaskViewDTO(task, namesUser);
                    tasksPages.Add(groupTaskViewDTO);
                }

                if (tasksPages.Count == 0) 
                {
                    return Ok(new StandartResponse<TasksPageDTO>
                    {
                        Message = "Tasks not found",
                        StatusCode = Domain.StatusCode.InternalServerError
                    });
                }

                var tasksPageDTO = new TasksPageDTO(tasksPages.ToArray(), isAdmin);

                return Ok(new StandartResponse<TasksPageDTO>
                {
                    Data = tasksPageDTO
                });
            }
            return Ok(new StandartResponse<GroupTask>
            {
                StatusCode = Domain.StatusCode.IdNotFound,
                Message = "Id not found or user not outorisation"
            });
        }
    }
}
