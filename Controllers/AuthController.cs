using HireAI.API.DTOs;
using HireAI.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HireAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = await _authService.Register(dto);

            if (user == null)
            {
                return BadRequest("Email already exists.");
            }

            return Ok(new
            {
                message = "User registered successfully."
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.Login(dto);

            if (token == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(new
            {
                token = token
            });
        }
    }
}