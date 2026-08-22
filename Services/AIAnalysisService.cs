
using HireAI.API.Data;
using HireAI.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HireAI.API.Services
{
    public class AIAnalysisService
    {
        private readonly ApplicationDbContext _context;

        public AIAnalysisService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AIAnalysis?> AnalyzeResume(int resumeId)
        {
            // Get the resume
            var resume = await _context.Resumes
                .FirstOrDefaultAsync(r => r.Id == resumeId);

            if (resume == null)
            {
                return null;
            }

            // Get extracted resume text
            string resumeText = resume.ExtractedText.ToLower();

            // Skills to detect
            var skills = new List<string>
            {
                "c",
                "c++",
                "c#",
                "java",
                "python",
                "javascript",
                "html",
                "css",
                "sql",
                "git",
                "github",
                "linux",
                "asp.net",
                "machine learning",
                "artificial intelligence",
                "data analysis",
                "react",
                "angular",
                "node.js"
            };

            // Find matched skills
            var matchedSkills = new List<string>();

            foreach (var skill in skills)
            {
                if (resumeText.Contains(skill.ToLower()))
                {
                    matchedSkills.Add(skill);
                }
            }

            // Calculate score
            int score = (int)Math.Round(
                (double)matchedSkills.Count / skills.Count * 100
            );

            // Find missing skills
            var missingSkills = skills
                .Where(skill => !matchedSkills.Contains(skill))
                .ToList();

            // Generate feedback
            string feedback;

            if (score >= 70)
            {
                feedback =
                    "The resume demonstrates a strong technical skill set.";
            }
            else if (score >= 40)
            {
                feedback =
                    "The resume demonstrates a moderate technical skill set, but several skills could be strengthened.";
            }
            else
            {
                feedback =
                    "The resume could benefit from adding more relevant technical skills and experience.";
            }

            // Check if an analysis already exists for this resume
            var analysis = await _context.AIAnalyses
                .FirstOrDefaultAsync(a => a.ResumeId == resumeId);

            if (analysis == null)
            {
                // Create new analysis
                analysis = new AIAnalysis
                {
                    ResumeId = resume.Id,
                    Score = score,
                    MatchedSkills = string.Join(", ", matchedSkills),
                    MissingSkills = string.Join(", ", missingSkills),
                    Feedback = feedback,
                    CreatedAt = DateTime.UtcNow
                };

                _context.AIAnalyses.Add(analysis);
            }
            else
            {
                // Update existing analysis
                analysis.Score = score;
                analysis.MatchedSkills = string.Join(", ", matchedSkills);
                analysis.MissingSkills = string.Join(", ", missingSkills);
                analysis.Feedback = feedback;
                analysis.CreatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return analysis;
        }
    }
}

