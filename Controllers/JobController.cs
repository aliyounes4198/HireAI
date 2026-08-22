using HireAI.API.Data;
using HireAI.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HireAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JobController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create a new job
        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> CreateJob(
            [FromBody] Job job)
        {
            // Get logged-in HR user ID from JWT
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

            // Find the HR user
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Unauthorized();
            }

            // Make sure HR belongs to a company
            if (user.CompanyId == null)
            {
                return BadRequest(
                    "HR user is not associated with a company."
                );
            }

            // Make sure the company actually exists
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == user.CompanyId.Value);

            if (company == null)
            {
                return BadRequest(
                    "The HR user's company does not exist."
                );
            }

            // Automatically assign the HR user's company
            job.CompanyId = user.CompanyId.Value;

            // Set job information
            job.CreatedAt = DateTime.UtcNow;
            job.Status = "Open";

            _context.Jobs.Add(job);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Job created successfully.",
                jobId = job.Id,
                title = job.Title,
                companyId = job.CompanyId,
                companyName = company.Name,
                status = job.Status
            });
        }

        // Get all jobs
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetJobs()
        {
            var jobs = _context.Jobs
                .Select(j => new
                {
                    j.Id,
                    j.Title,
                    j.Description,
                    j.Requirements,
                    j.Location,
                    j.Status,
                    j.CreatedAt,
                    j.CompanyId,

                    companyName = j.Company != null
                        ? j.Company.Name
                        : "Unknown"
                })
                .ToList();

            return Ok(jobs);
        }

        // Get only open jobs
        [HttpGet("open")]
        [AllowAnonymous]
        public IActionResult GetOpenJobs()
        {
            var jobs = _context.Jobs
                .Where(j => j.Status == "Open")
                .Select(j => new
                {
                    j.Id,
                    j.Title,
                    j.Description,
                    j.Requirements,
                    j.Location,
                    j.Status,
                    j.CreatedAt,
                    j.CompanyId,

                    companyName = j.Company != null
                        ? j.Company.Name
                        : "Unknown"
                })
                .ToList();

            return Ok(jobs);
        }

        // Get one job
        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetJob(int id)
        {
            var job = _context.Jobs
                .Where(j => j.Id == id)
                .Select(j => new
                {
                    j.Id,
                    j.Title,
                    j.Description,
                    j.Requirements,
                    j.Location,
                    j.Status,
                    j.CreatedAt,
                    j.CompanyId,

                    companyName = j.Company != null
                        ? j.Company.Name
                        : "Unknown"
                })
                .FirstOrDefault();

            if (job == null)
            {
                return NotFound("Job not found.");
            }

            return Ok(job);
        }
    }
}