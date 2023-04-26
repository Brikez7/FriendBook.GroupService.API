using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FriendBook.GroupService.API.Controllers
{
    public class GroupODataController : ODataController
    {
        private readonly IGroupService _groupService;

        public GroupODataController(IGroupService relationshipService)
        {
            _groupService = relationshipService;
        }

        [HttpGet("odata/v1/Group")]
        [EnableQuery]
        public IQueryable<Group> GetGroup()
        {

            return _groupService.GetGroupOData().Data;
        }
    }
}