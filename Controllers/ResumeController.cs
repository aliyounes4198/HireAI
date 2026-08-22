using HireAI.API.Data;
using HireAI.API.Models;
using UglyToad.PdfPig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HireAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Candidate")]
    public class ResumeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;


    public ResumeController(
        ApplicationDbContext context,
        IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // POST: api/Resume/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadResume(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please select a file.");
            }

            // Only allow PDF files
            if (!Path.GetExtension(file.FileName)
                .Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only PDF files are allowed.");
            }

            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var candidate = await _context.CandidateProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (candidate == null)
            {
                return BadRequest(
                    "Candidate profile not found. Create your profile first."
                );
            }

            // Create upload folder
            var uploadsFolder = Path.Combine(
                _environment.ContentRootPath,
                "Uploads",
                "Resumes"
            );

            Directory.CreateDirectory(uploadsFolder);

            // Generate unique filename
            var uniqueFileName =
                $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

            var filePath = Path.Combine(
                uploadsFolder,
                uniqueFileName
            );

            // Save PDF
            using (var stream = new FileStream(
                filePath,
                FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Extract PDF text
            string extractedText = "";

            try
            {
                using (var document = PdfDocument.Open(filePath))
                {
                    foreach (var page in document.GetPages())
                    {
                        extractedText += page.Text + "\n";
                    }
                }
            }
            catch
            {
                // Delete the uploaded file if PDF extraction fails
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                return BadRequest(
                    "The PDF could not be read."
                );
            }

            // Create database record
            var resume = new Resume
            {
                CandidateId = candidate.Id,
                FileName = file.FileName,
                FilePath = filePath,
                ExtractedText = extractedText,
                UploadDate = DateTime.UtcNow
            };

            _context.Resumes.Add(resume);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Resume uploaded and text extracted successfully.",
                resumeId = resume.Id,
                fileName = resume.FileName,
                extractedText = resume.ExtractedText
            });
        }

        // GET: api/Resume
        [HttpGet]
        public async Task<IActionResult> GetResumes()
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var candidate = await _context.CandidateProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (candidate == null)
            {
                return NotFound("Candidate profile not found.");
            }

            var resumes = await _context.Resumes
                .Where(r => r.CandidateId == candidate.Id)
                .Select(r => new
                {
                    r.Id,
                    r.FileName,
                    r.UploadDate,
                    r.ExtractedText
                })
                .ToListAsync();

            return Ok(resumes);
        }

        // DELETE: api/Resume/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResume(int id)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var candidate = await _context.CandidateProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (candidate == null)
            {
                return NotFound("Candidate profile not found.");
            }

            var resume = await _context.Resumes
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.CandidateId == candidate.Id);

            if (resume == null)
            {
                return NotFound("Resume not found.");
            }

            if (System.IO.File.Exists(resume.FilePath))
            {
                System.IO.File.Delete(resume.FilePath);
            }

            _context.Resumes.Remove(resume);

            await _context.SaveChangesAsync();

            return Ok("Resume deleted successfully.");
        }
    }


}
