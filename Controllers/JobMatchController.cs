using HireAI.API.Data;
using HireAI.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HireAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobMatchController : ControllerBase
    {
        private readonly JobMatchService _jobMatchService;
        private readonly ApplicationDbContext _context;

        public JobMatchController(
            JobMatchService jobMatchService,
            ApplicationDbContext context)
        {
            _jobMatchService = jobMatchService;
            _context = context;
        }


        // =====================================================
        // Match a resume to a job
        // =====================================================

        [HttpPost("{resumeId}/{jobId}")]
        public async Task<IActionResult> MatchResumeToJob(
            int resumeId,
            int jobId)
        {
            var match = await _jobMatchService.MatchResumeToJob(
                resumeId,
                jobId
            );

            if (match == null)
            {
                return NotFound("Resume or Job not found.");
            }

            return Ok(new
            {
                message = "Resume matched with job successfully.",

                matchId = match.Id,

                resumeId = match.ResumeId,

                jobId = match.JobId,

                matchScore = match.MatchScore,

                matchedSkills = match.MatchedSkills,

                missingSkills = match.MissingSkills,

                experienceRelevance =
                    match.ExperienceRelevance,

                educationRelevance =
                    match.EducationRelevance,

                strengths =
                    match.Strengths,

                weaknesses =
                    match.Weaknesses,

                recommendation =
                    match.Recommendation,

                createdAt =
                    match.CreatedAt
            });
        }


        // =====================================================
        // Get job matches for HR user's company
        // =====================================================

        [HttpGet("company")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetCompanyJobMatches()
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(
                userIdClaim.Value,
                out int userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Id == userId);

            if (user == null)
            {
                return Unauthorized();
            }

            if (user.CompanyId == null)
            {
                return BadRequest(
                    "HR user is not associated with a company."
                );
            }

            var matches = await _context.JobMatches
                .Where(m =>
                    m.Job.CompanyId ==
                    user.CompanyId)

                .Select(m => new
                {
                    id = m.Id,

                    resumeId = m.ResumeId,

                    resumeFileName =
                        m.Resume.FileName,

                    jobId = m.JobId,

                    jobTitle =
                        m.Job.Title,

                    companyId =
                        m.Job.CompanyId,

                    matchScore =
                        m.MatchScore,

                    matchedSkills =
                        m.MatchedSkills,

                    missingSkills =
                        m.MissingSkills,

                    experienceRelevance =
                        m.ExperienceRelevance,

                    educationRelevance =
                        m.EducationRelevance,

                    strengths =
                        m.Strengths,

                    weaknesses =
                        m.Weaknesses,

                    recommendation =
                        m.Recommendation,

                    createdAt =
                        m.CreatedAt
                })

                .ToListAsync();

            return Ok(matches);
        }
    }
}