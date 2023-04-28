using FriendBook.GroupService.API.BLL.Interfaces;
using FriendBook.GroupService.API.BLL.Services;
using FriendBook.GroupService.API.Domain.Entities;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;

namespace FriendBook.GroupService.API.Controllers
{
    public class AccountStatusGroupODataController : Controller
    {
        private readonly IAccountStatusGroupService _accountStatusGroupService;

        public AccountStatusGroupODataController(IAccountStatusGroupService accountStatusGroupService)
        {
            _accountStatusGroupService = accountStatusGroupService;
        }

        [HttpGet("OData/Groups")]
        [EnableQuery]
        public IQueryable<AccountStatusGroup> GetGroups()
        {
            return _accountStatusGroupService.GetAccountStatusGroupOData().Data;
        }
    }
}
