namespace HireAI.API.Models
{
    public class AIAnalysis
    {
        public int Id { get; set; }

        public int ResumeId { get; set; }

        public int Score { get; set; }

        public string MatchedSkills { get; set; } = string.Empty;

        public string MissingSkills { get; set; } = string.Empty;

        public string Feedback { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Navigation property

        public Resume Resume { get; set; } = null!;
    }
}