using HireAI.API.Data;
using HireAI.API.DTOs;
using HireAI.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HireAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Candidate")]
    public class CandidateProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;


    public CandidateProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CandidateProfile
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var profile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                return NotFound("Candidate profile not found.");
            }

            return Ok(profile);
        }

        // POST: api/CandidateProfile
        [HttpPost]
        public async Task<IActionResult> CreateProfile(
            CandidateProfileDto dto)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var existingProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (existingProfile != null)
            {
                return BadRequest("Candidate profile already exists.");
            }

            var profile = new CandidateProfile
            {
                UserId = userId,
                Phone = dto.Phone,
                Skills = dto.Skills,
                Education = dto.Education,
                Experience = dto.Experience,
                LinkedIn = dto.LinkedIn,
                GitHub = dto.GitHub
            };

            _context.CandidateProfiles.Add(profile);

            await _context.SaveChangesAsync();

            return Ok(profile);
        }

        // PUT: api/CandidateProfile
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(
            CandidateProfileDto dto)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var profile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                return NotFound("Candidate profile not found.");
            }

            profile.Phone = dto.Phone;
            profile.Skills = dto.Skills;
            profile.Education = dto.Education;
            profile.Experience = dto.Experience;
            profile.LinkedIn = dto.LinkedIn;
            profile.GitHub = dto.GitHub;

            await _context.SaveChangesAsync();

            return Ok(profile);
        }
    }


}
