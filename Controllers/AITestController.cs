using HireAI.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AITestController : ControllerBase
    {
        private readonly OllamaService _ollamaService;

        public AITestController(OllamaService ollamaService)
        {
            _ollamaService = ollamaService;
        }

        [HttpPost]
        public async Task<IActionResult> TestAI([FromBody] string prompt)
        {
            try
            {
                var response = await _ollamaService.AskAI(prompt);

                return Ok(new
                {
                    message = "AI connection successful.",
                    response = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "AI connection failed.",
                    error = ex.Message
                });
            }
        }
    }
}