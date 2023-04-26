using Microsoft.AspNetCore.Mvc;

namespace FriendBook.GroupService.API.Controllers
{
    public class GroupController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
