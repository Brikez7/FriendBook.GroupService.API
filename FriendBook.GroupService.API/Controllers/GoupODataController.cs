using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FriendBook.GroupService.API.Controllers
{
    [Route("api/GroupOData[controller]")]
    public class GroupODataController : ODataController
    {
        private readonly IGroupService _groupService;

        public GroupODataController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet("OData/Groups")]
        [EnableQuery]
        public IQueryable<Group> GetGroups()
        {
            return _groupService.GetGroupOData().Data;
        }
    }
}