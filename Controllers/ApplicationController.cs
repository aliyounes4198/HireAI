using HireAI.API.Data;
using HireAI.API.Models;
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
    public class ApplicationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JobMatchService _jobMatchService;

        public ApplicationController(
            ApplicationDbContext context,
            JobMatchService jobMatchService)
        {
            _context = context;
            _jobMatchService = jobMatchService;
        }

        // Candidate applies for a job
        // AI matching happens automatically
        [HttpPost]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> CreateApplication(
            [FromBody] Application application)
        {
            // Get logged-in user ID from JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            // Find candidate profile belonging to logged-in user
            var candidate = await _context.CandidateProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (candidate == null)
            {
                return BadRequest("Candidate profile not found.");
            }

            // Find job
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == application.JobId);

            if (job == null)
            {
                return BadRequest("Job not found.");
            }

            // Make sure resume belongs to this candidate
            var resume = await _context.Resumes
                .FirstOrDefaultAsync(r =>
                    r.Id == application.ResumeId &&
                    r.CandidateId == candidate.Id);

            if (resume == null)
            {
                return BadRequest(
                    "Resume not found or does not belong to you."
                );
            }

            // Prevent duplicate application
            var existingApplication = await _context.Applications
                .FirstOrDefaultAsync(a =>
                    a.CandidateId == candidate.Id &&
                    a.JobId == application.JobId);

            if (existingApplication != null)
            {
                return BadRequest(
                    "You have already applied for this job."
                );
            }

            // Force the correct candidate
            application.CandidateId = candidate.Id;

            application.AppliedDate = DateTime.UtcNow;
            application.Status = "Pending";

            _context.Applications.Add(application);

            await _context.SaveChangesAsync();

            // Run AI matching automatically
            var match = await _jobMatchService.MatchResumeToJob(
                resume.Id,
                job.Id
            );

            return Ok(new
            {
                message = "Application submitted successfully.",
                applicationId = application.Id,

                jobId = job.Id,
                jobTitle = job.Title,

                resumeId = resume.Id,

                status = application.Status,

                aiMatch = match == null
                    ? null
                    : new
                    {
                        matchId = match.Id,
                        matchScore = match.MatchScore,
                        matchedSkills = match.MatchedSkills,
                        missingSkills = match.MissingSkills,
                        experienceRelevance =
                            match.ExperienceRelevance,
                        educationRelevance =
                            match.EducationRelevance,
                        strengths = match.Strengths,
                        weaknesses = match.Weaknesses,
                        recommendation =
                            match.Recommendation
                    }
            });
        }

        // Get all applications
        [HttpGet]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetApplications()
        {
            var applications = await _context.Applications
                .Select(a => new
                {
                    id = a.Id,

                    candidateId = a.CandidateId,
                    candidateName = a.Candidate.User.FullName,

                    jobId = a.JobId,
                    jobTitle = a.Job.Title,

                    resumeId = a.ResumeId,
                    resumeFileName = a.Resume.FileName,

                    status = a.Status,
                    appliedDate = a.AppliedDate
                })
                .ToListAsync();

            return Ok(applications);
        }

        // Get one application
        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplication(int id)
        {
            var application = await _context.Applications
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    id = a.Id,

                    candidateId = a.CandidateId,
                    candidateName = a.Candidate.User.FullName,

                    jobId = a.JobId,
                    jobTitle = a.Job.Title,

                    resumeId = a.ResumeId,
                    resumeFileName = a.Resume.FileName,

                    status = a.Status,
                    appliedDate = a.AppliedDate
                })
                .FirstOrDefaultAsync();

            if (application == null)
            {
                return NotFound("Application not found.");
            }

            return Ok(application);
        }

        // Get applications for the HR user's company
        [HttpGet("company")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> GetCompanyApplications()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

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

            var applications = await _context.Applications
                .Where(a =>
                    a.Job.CompanyId == user.CompanyId)
                .Select(a => new
                {
                    id = a.Id,

                    candidateId = a.CandidateId,
                    candidateName =
                        a.Candidate.User.FullName,

                    jobId = a.JobId,
                    jobTitle = a.Job.Title,

                    resumeId = a.ResumeId,
                    resumeFileName = a.Resume.FileName,

                    status = a.Status,
                    appliedDate = a.AppliedDate
                })
                .ToListAsync();

            return Ok(applications);
        }

        // Get applications belonging to logged-in candidate
        [HttpGet("my-applications")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> GetMyApplications()
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            var candidate = await _context.CandidateProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (candidate == null)
            {
                return BadRequest(
                    "Candidate profile not found."
                );
            }

            var applications = await _context.Applications
                .Where(a => a.CandidateId == candidate.Id)
                .Select(a => new
                {
                    id = a.Id,

                    jobId = a.JobId,
                    jobTitle = a.Job.Title,

                    companyId = a.Job.CompanyId,
                    companyName = a.Job.Company.Name,

                    resumeId = a.ResumeId,
                    resumeFileName = a.Resume.FileName,

                    status = a.Status,
                    appliedDate = a.AppliedDate
                })
                .ToListAsync();

            return Ok(applications);
        }

        // HR updates application status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> UpdateApplicationStatus(
            int id,
            [FromBody] string status)
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

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

            var application = await _context.Applications
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
            {
                return NotFound("Application not found.");
            }

            // Make sure application belongs to HR's company
            if (application.Job.CompanyId != user.CompanyId.Value)
            {
                return Forbid();
            }

            var allowedStatuses = new[]
            {
                "Pending",
                "Reviewed",
                "Accepted",
                "Rejected"
            };

            if (!allowedStatuses.Contains(status))
            {
                return BadRequest(
                    "Invalid status. Use Pending, Reviewed, Accepted, or Rejected."
                );
            }

            application.Status = status;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message =
                    "Application status updated successfully.",

                applicationId = application.Id,

                status = application.Status
            });
        }
    }
}