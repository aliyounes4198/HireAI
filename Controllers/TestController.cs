using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok("You are authenticated!");
        }


    [HttpGet("candidate")]
        [Authorize(Roles = "Candidate")]
        public IActionResult Candidate()
        {
            return Ok("You are a Candidate!");
        }

        [HttpGet("hr")]
        [Authorize(Roles = "HR")]
        public IActionResult HR()
        {
            return Ok("You are HR!");
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return Ok("You are an Admin!");
        }
    }


}
