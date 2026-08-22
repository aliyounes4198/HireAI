namespace HireAI.API.Models
{
    public class AIJobMatchResult
    {
        public int Score { get; set; }

        public List<string> MatchedSkills { get; set; } = new();

        public List<string> MissingSkills { get; set; } = new();

        public bool ExperienceRelevance { get; set; }

        public bool EducationRelevance { get; set; }

        public List<string> Strengths { get; set; } = new();

        public List<string> Weaknesses { get; set; } = new();

        public string Recommendation { get; set; } = string.Empty;
    }
}