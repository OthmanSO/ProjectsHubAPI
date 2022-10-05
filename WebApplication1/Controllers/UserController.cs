using Microsoft.AspNetCore.Mvc;

namespace ProjectHubAPI.Controllers
{
    [ApiController]
    [Route("/api/V1.0/usres")]
    public class UserController : ControllerBase
    {
        [HttpGet("/{id}")]
        public ActionResult<object> GetUser(Guid id)
        {
            return null;
        }
    }
}
