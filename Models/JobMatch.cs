namespace HireAI.API.Models
{
    public class JobMatch
    {
        public int Id { get; set; }

        public int ResumeId { get; set; }

        public int JobId { get; set; }

        public int MatchScore { get; set; }

        public string MatchedSkills { get; set; } = string.Empty;

        public string MissingSkills { get; set; } = string.Empty;

        public bool ExperienceRelevance { get; set; }

        public bool EducationRelevance { get; set; }

        public string Strengths { get; set; } = string.Empty;

        public string Weaknesses { get; set; } = string.Empty;

        public string Recommendation { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties

        public Resume Resume { get; set; } = null!;

        public Job Job { get; set; } = null!;
    }
}