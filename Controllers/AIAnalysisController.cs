using HireAI.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AIAnalysisController : ControllerBase
    {
        private readonly AIAnalysisService _aiAnalysisService;

        public AIAnalysisController(AIAnalysisService aiAnalysisService)
        {
            _aiAnalysisService = aiAnalysisService;
        }

        [HttpPost("{resumeId}")]
        public async Task<IActionResult> AnalyzeResume(int resumeId)
        {
            var analysis = await _aiAnalysisService.AnalyzeResume(resumeId);

            if (analysis == null)
            {
                return NotFound("Resume not found.");
            }

            return Ok(new
            {
                message = "Resume analyzed successfully.",
                analysisId = analysis.Id,
                resumeId = analysis.ResumeId,
                score = analysis.Score,
                matchedSkills = analysis.MatchedSkills,
                missingSkills = analysis.MissingSkills,
                feedback = analysis.Feedback
            });
        }
    }
}