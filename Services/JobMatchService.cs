using System.Text.Json;
using HireAI.API.Data;
using HireAI.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HireAI.API.Services
{
    public class JobMatchService
    {
        private readonly ApplicationDbContext _context;
        private readonly OllamaService _ollamaService;

        public JobMatchService(
            ApplicationDbContext context,
            OllamaService ollamaService)
        {
            _context = context;
            _ollamaService = ollamaService;
        }

        public async Task<JobMatch?> MatchResumeToJob(
            int resumeId,
            int jobId)
        {
            // =====================================================
            // GET RESUME
            // =====================================================

            var resume = await _context.Resumes
                .FirstOrDefaultAsync(r => r.Id == resumeId);

            if (resume == null)
            {
                return null;
            }

            // =====================================================
            // GET JOB
            // =====================================================

            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
            {
                return null;
            }

            string resumeText =
                resume.ExtractedText ?? string.Empty;

            string jobTitle =
                job.Title ?? string.Empty;

            string jobDescription =
                job.Description ?? string.Empty;

            string requirements =
                job.Requirements ?? string.Empty;

            // =====================================================
            // NORMALIZE TEXT
            // =====================================================

            string resumeLower =
                NormalizeText(resumeText);

            string requirementsLower =
                NormalizeText(requirements);

            // =====================================================
            // SKILL ALIASES
            // =====================================================

            var skillAliases =
                new Dictionary<string, string[]>(
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        "C#",
                        new[]
                        {
                            "c#",
                            "c sharp"
                        }
                    },

                    {
                        "ASP.NET Core",
                        new[]
                        {
                            "asp.net core",
                            "aspnet core",
                            "asp.net core web api"
                        }
                    },

                    {
                        "SQL",
                        new[]
                        {
                            "sql",
                            "sql server",
                            "sql queries",
                            "database management systems & sql"
                        }
                    },

                    {
                        "Entity Framework",
                        new[]
                        {
                            "entity framework",
                            "entity framework core",
                            "ef core"
                        }
                    },

                    {
                        "Git",
                        new[]
                        {
                            "git",
                            "github"
                        }
                    },

                    {
                        "REST API",
                        new[]
                        {
                            "rest api",
                            "restful api",
                            "restful web api",
                            "api creation",
                            "backend api",
                            "backend apis"
                        }
                    }
                };

            // =====================================================
            // FIND REQUIRED SKILLS
            // =====================================================

            var requiredSkills =
                new List<string>();

            foreach (var skill in skillAliases)
            {
                bool required =
                    skill.Value.Any(alias =>
                        ContainsSkill(
                            requirementsLower,
                            alias));

                if (required)
                {
                    requiredSkills.Add(skill.Key);
                }
            }

            // =====================================================
            // FIND MATCHED / MISSING SKILLS
            // =====================================================

            var matchedSkills =
                new List<string>();

            var missingSkills =
                new List<string>();

            foreach (var skill in requiredSkills)
            {
                var aliases =
                    skillAliases[skill];

                bool foundInResume =
                    aliases.Any(alias =>
                        ContainsSkill(
                            resumeLower,
                            alias));

                if (foundInResume)
                {
                    matchedSkills.Add(skill);
                }
                else
                {
                    missingSkills.Add(skill);
                }
            }

            // =====================================================
            // CALCULATE SCORE
            // =====================================================

            int score;

            if (requiredSkills.Count == 0)
            {
                score = 0;
            }
            else
            {
                score =
                    (int)Math.Round(
                        (double)matchedSkills.Count /
                        requiredSkills.Count *
                        100);
            }

            // =====================================================
            // EXPERIENCE RELEVANCE
            // =====================================================

            bool experienceRelevance =
                HasRelevantExperience(
                    resumeLower);

            // =====================================================
            // EDUCATION RELEVANCE
            // =====================================================

            bool educationRelevance =
                resumeLower.Contains(
                    "computer science");

            // =====================================================
            // DEFAULT STRENGTHS
            // =====================================================

            var defaultStrengths =
                new List<string>();

            if (resumeLower.Contains("problem solving") ||
                resumeLower.Contains("problem-solving"))
            {
                defaultStrengths.Add(
                    "Problem-solving");
            }

            if (resumeLower.Contains("debugging"))
            {
                defaultStrengths.Add(
                    "Debugging");
            }

            if (resumeLower.Contains("teamwork"))
            {
                defaultStrengths.Add(
                    "Teamwork");
            }

            if (resumeLower.Contains("communication"))
            {
                defaultStrengths.Add(
                    "Communication");
            }

            if (resumeLower.Contains("fast learning"))
            {
                defaultStrengths.Add(
                    "Fast learning");
            }

            if (resumeLower.Contains("backend"))
            {
                defaultStrengths.Add(
                    "Backend development experience");
            }

            if (resumeLower.Contains("api"))
            {
                defaultStrengths.Add(
                    "API development experience");
            }

            if (resumeLower.Contains("sql server"))
            {
                defaultStrengths.Add(
                    "Database development experience");
            }

            // Remove duplicates
            defaultStrengths =
                CleanList(defaultStrengths);

            // =====================================================
            // DEFAULT WEAKNESSES
            // =====================================================

            var defaultWeaknesses =
                new List<string>();

            foreach (var missingSkill in missingSkills)
            {
                defaultWeaknesses.Add(
                    $"Missing {missingSkill} experience");
            }

            // =====================================================
            // DEFAULT RECOMMENDATION
            // =====================================================

            string defaultRecommendation;

            if (score >= 80)
            {
                defaultRecommendation =
                    "The candidate is an excellent match for the position and meets most or all required skills.";
            }
            else if (score >= 60)
            {
                defaultRecommendation =
                    "The candidate is a good match for the position but may need additional development in some required skills.";
            }
            else if (score >= 40)
            {
                defaultRecommendation =
                    "The candidate is a moderate match and would benefit from strengthening several required skills.";
            }
            else
            {
                defaultRecommendation =
                    "The candidate is currently a limited match for the position because several required skills are missing.";
            }

            // =====================================================
            // ASK OLLAMA FOR EXPLANATION
            // =====================================================

            var prompt = $@"
You are an AI recruitment assistant.

Analyze the candidate for the following job.

JOB TITLE:
{jobTitle}

JOB DESCRIPTION:
{jobDescription}

JOB REQUIREMENTS:
{requirements}

RESUME:
{resumeText}

The backend has already calculated the factual matching information.

MATCH SCORE:
{score}

MATCHED SKILLS:
{string.Join(", ", matchedSkills)}

MISSING SKILLS:
{string.Join(", ", missingSkills)}

EXPERIENCE RELEVANCE:
{experienceRelevance}

EDUCATION RELEVANCE:
{educationRelevance}

Your job is ONLY to generate:

1. strengths
2. weaknesses
3. recommendation

IMPORTANT RULES:

- Do NOT change the score.
- Do NOT add matched skills.
- Do NOT remove matched skills.
- Do NOT add missing skills.
- Do NOT remove missing skills.
- Do NOT invent information.
- Use ONLY information contained in the resume.
- Strengths must be relevant to the job.
- Weaknesses must only describe actual missing required skills.
- Do not mention Python, Java, JavaScript, React, Docker, AWS, Azure,
  C++, or any other technology unless it is explicitly required by the job.
- Do not say the candidate lacks a skill if that skill is in the matched skills.
- If there are no weaknesses, return [].
- If there are no strengths, return [].
- The recommendation must be consistent with the score.
- Do not say the candidate is missing a skill that appears in the resume.

Return ONLY valid JSON.

The JSON MUST contain exactly these properties:

{{
  ""strengths"": [],
  ""weaknesses"": [],
  ""recommendation"": """"
}}

Example:

{{
  ""strengths"": [
    ""Backend development experience"",
    ""Strong database knowledge""
  ],
  ""weaknesses"": [
    ""Limited experience with REST API""
  ],
  ""recommendation"": ""The candidate is a good match for the position.""
}}
";

            // =====================================================
            // AI RESULT DEFAULTS
            // =====================================================

            List<string> strengths =
                defaultStrengths;

            List<string> weaknesses =
                defaultWeaknesses;

            string recommendation =
                defaultRecommendation;

            try
            {
                // =================================================
                // ASK OLLAMA
                // =================================================

                string aiResponse =
                    await _ollamaService.AskAI(prompt);

                aiResponse =
                    CleanJsonResponse(aiResponse);

                // =================================================
                // DESERIALIZE AI RESPONSE
                // =================================================

                var aiResult =
                    JsonSerializer.Deserialize<AIJobMatchResult>(
                        aiResponse,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (aiResult != null)
                {
                    // Only accept AI explanation fields.
                    // Backend keeps score and skills.
                    if (aiResult.Strengths != null &&
                        aiResult.Strengths.Count > 0)
                    {
                        strengths =
                            CleanList(
                                aiResult.Strengths);
                    }

                    if (aiResult.Weaknesses != null &&
                        aiResult.Weaknesses.Count > 0)
                    {
                        weaknesses =
                            CleanList(
                                aiResult.Weaknesses);
                    }

                    if (!string.IsNullOrWhiteSpace(
                        aiResult.Recommendation))
                    {
                        recommendation =
                            aiResult.Recommendation.Trim();
                    }
                }
            }
            catch
            {
                // If Ollama fails, use the safe
                // backend-generated values.
                strengths =
                    defaultStrengths;

                weaknesses =
                    defaultWeaknesses;

                recommendation =
                    defaultRecommendation;
            }

            // =====================================================
            // FINAL VALIDATION OF WEAKNESSES
            // =====================================================

            weaknesses =
                ValidateWeaknesses(
                    weaknesses,
                    missingSkills);

            // If AI removed everything, use backend defaults.
            if (weaknesses.Count == 0 &&
                missingSkills.Count > 0)
            {
                weaknesses =
                    defaultWeaknesses;
            }

            // =====================================================
            // FINAL VALIDATION OF STRENGTHS
            // =====================================================

            strengths =
                CleanList(strengths);

            // =====================================================
            // FIND EXISTING MATCH
            // =====================================================

            var jobMatch =
                await _context.JobMatches
                    .FirstOrDefaultAsync(m =>
                        m.ResumeId == resumeId &&
                        m.JobId == jobId);

            // =====================================================
            // CREATE MATCH IF NECESSARY
            // =====================================================

            if (jobMatch == null)
            {
                jobMatch =
                    new JobMatch
                    {
                        ResumeId = resumeId,
                        JobId = jobId
                    };

                _context.JobMatches.Add(jobMatch);
            }

            // =====================================================
            // SAVE FINAL RESULT
            // =====================================================

            jobMatch.MatchScore =
                score;

            jobMatch.MatchedSkills =
                string.Join(
                    ", ",
                    matchedSkills);

            jobMatch.MissingSkills =
                string.Join(
                    ", ",
                    missingSkills);

            jobMatch.ExperienceRelevance =
                experienceRelevance;

            jobMatch.EducationRelevance =
                educationRelevance;

            jobMatch.Strengths =
                string.Join(
                    ", ",
                    strengths);

            jobMatch.Weaknesses =
                string.Join(
                    ", ",
                    weaknesses);

            jobMatch.Recommendation =
                recommendation;

            jobMatch.CreatedAt =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return jobMatch;
        }

        // =====================================================
        // CHECK SKILL
        // =====================================================

        private bool ContainsSkill(
            string text,
            string skill)
        {
            string normalizedSkill =
                NormalizeText(skill);

            return text.Contains(
                normalizedSkill,
                StringComparison.OrdinalIgnoreCase);
        }

        // =====================================================
        // NORMALIZE TEXT
        // =====================================================

        private string NormalizeText(
            string text)
        {
            return text
                .ToLowerInvariant()
                .Replace("–", "-")
                .Replace("—", "-")
                .Trim();
        }

        // =====================================================
        // EXPERIENCE RELEVANCE
        // =====================================================

        private bool HasRelevantExperience(
            string resumeText)
        {
            string[] relevantTerms =
            {
                "backend development",
                "backend",
                "software development",
                "web development",
                "api",
                "developer",
                "programming",
                "asp.net core",
                "entity framework",
                "sql server"
            };

            return relevantTerms.Any(
                term =>
                    resumeText.Contains(
                        term,
                        StringComparison.OrdinalIgnoreCase));
        }

        // =====================================================
        // CLEAN AI LIST
        // =====================================================

        private List<string> CleanList(
            List<string>? values)
        {
            if (values == null)
            {
                return new List<string>();
            }

            return values
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .Select(x =>
                    x.Trim())
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // =====================================================
        // VALIDATE WEAKNESSES
        // =====================================================

        private List<string> ValidateWeaknesses(
            List<string> weaknesses,
            List<string> missingSkills)
        {
            if (weaknesses == null ||
                weaknesses.Count == 0)
            {
                return new List<string>();
            }

            if (missingSkills == null ||
                missingSkills.Count == 0)
            {
                return new List<string>();
            }

            var validated =
                new List<string>();

            foreach (var weakness in weaknesses)
            {
                string weaknessLower =
                    weakness.ToLowerInvariant();

                bool refersToMissingSkill =
                    missingSkills.Any(
                        skill =>
                            weaknessLower.Contains(
                                skill.ToLowerInvariant()));

                if (refersToMissingSkill)
                {
                    validated.Add(
                        weakness);
                }
            }

            return CleanList(validated);
        }

        // =====================================================
        // CLEAN JSON RESPONSE
        // =====================================================

        private string CleanJsonResponse(
            string response)
        {
            if (string.IsNullOrWhiteSpace(response))
            {
                throw new Exception(
                    "Ollama returned an empty response.");
            }

            response =
                response.Trim();

            // Remove Markdown code fences
            if (response.StartsWith("```"))
            {
                response =
                    response
                        .Replace("```json", "")
                        .Replace("```JSON", "")
                        .Replace("```", "")
                        .Trim();
            }

            // Find first JSON object
            int start =
                response.IndexOf('{');

            int end =
                response.LastIndexOf('}');

            if (start >= 0 &&
                end > start)
            {
                response =
                    response.Substring(
                        start,
                        end - start + 1);
            }

            return response.Trim();
        }

        // =====================================================
        // RULE-BASED FALLBACK
        // =====================================================

        private async Task<JobMatch> RuleBasedMatch(
            Resume resume,
            Job job)
        {
            string resumeText =
                NormalizeText(
                    resume.ExtractedText ??
                    string.Empty);

            string requirements =
                NormalizeText(
                    job.Requirements ??
                    string.Empty);

            var skills =
                new Dictionary<string, string[]>(
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        "C#",
                        new[]
                        {
                            "c#",
                            "c sharp"
                        }
                    },

                    {
                        "ASP.NET Core",
                        new[]
                        {
                            "asp.net core",
                            "aspnet core"
                        }
                    },

                    {
                        "SQL",
                        new[]
                        {
                            "sql",
                            "sql server",
                            "sql queries"
                        }
                    },

                    {
                        "Entity Framework",
                        new[]
                        {
                            "entity framework",
                            "entity framework core",
                            "ef core"
                        }
                    },

                    {
                        "Git",
                        new[]
                        {
                            "git",
                            "github"
                        }
                    },

                    {
                        "REST API",
                        new[]
                        {
                            "rest api",
                            "restful api",
                            "restful web api",
                            "api creation",
                            "backend api",
                            "backend apis"
                        }
                    }
                };

            // =================================================
            // REQUIRED SKILLS
            // =================================================

            var requiredSkills =
                skills
                    .Where(skill =>
                        skill.Value.Any(alias =>
                            ContainsSkill(
                                requirements,
                                alias)))
                    .Select(skill =>
                        skill.Key)
                    .ToList();

            // =================================================
            // MATCHED / MISSING
            // =================================================

            var matchedSkills =
                new List<string>();

            var missingSkills =
                new List<string>();

            foreach (var skill in requiredSkills)
            {
                bool found =
                    skills[skill].Any(alias =>
                        ContainsSkill(
                            resumeText,
                            alias));

                if (found)
                {
                    matchedSkills.Add(skill);
                }
                else
                {
                    missingSkills.Add(skill);
                }
            }

            // =================================================
            // SCORE
            // =================================================

            int score =
                requiredSkills.Count == 0
                    ? 0
                    : (int)Math.Round(
                        (double)matchedSkills.Count /
                        requiredSkills.Count *
                        100);

            // =================================================
            // RECOMMENDATION
            // =================================================

            string recommendation;

            if (score >= 80)
            {
                recommendation =
                    "The candidate is an excellent match for the position.";
            }
            else if (score >= 60)
            {
                recommendation =
                    "The candidate is a good match for the position but may need additional development in some required skills.";
            }
            else if (score >= 40)
            {
                recommendation =
                    "The candidate is a moderate match and would benefit from strengthening several required skills.";
            }
            else
            {
                recommendation =
                    "The candidate is currently a limited match because several required skills are missing.";
            }

            // =================================================
            // FIND EXISTING MATCH
            // =================================================

            var jobMatch =
                await _context.JobMatches
                    .FirstOrDefaultAsync(m =>
                        m.ResumeId == resume.Id &&
                        m.JobId == job.Id);

            if (jobMatch == null)
            {
                jobMatch =
                    new JobMatch
                    {
                        ResumeId = resume.Id,
                        JobId = job.Id
                    };

                _context.JobMatches.Add(jobMatch);
            }

            // =================================================
            // SAVE FALLBACK
            // =================================================

            jobMatch.MatchScore =
                score;

            jobMatch.MatchedSkills =
                string.Join(
                    ", ",
                    matchedSkills);

            jobMatch.MissingSkills =
                string.Join(
                    ", ",
                    missingSkills);

            jobMatch.ExperienceRelevance =
                HasRelevantExperience(
                    resumeText);

            jobMatch.EducationRelevance =
                resumeText.Contains(
                    "computer science");

            jobMatch.Strengths =
                string.Empty;

            jobMatch.Weaknesses =
                string.Join(
                    ", ",
                    missingSkills.Select(
                        skill =>
                            $"Missing {skill} experience"));

            jobMatch.Recommendation =
                recommendation;

            jobMatch.CreatedAt =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return jobMatch;
        }
    }
}