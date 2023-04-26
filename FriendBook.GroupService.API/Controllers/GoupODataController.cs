using Microsoft.AspNetCore.Mvc;

namespace FriendBook.GroupService.API.Controllers
{
    [ApiController]
    [Route("group/[controller]")]
    public class GoupODataController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<GoupODataController> _logger;

        public GoupODataController(ILogger<GoupODataController> logger)
        {
            _logger = logger;
        }
    }
}