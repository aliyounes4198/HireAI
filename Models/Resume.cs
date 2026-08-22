using static System.Net.Mime.MediaTypeNames;

namespace HireAI.API.Models
{
    public class Resume
    {
        public int Id { get; set; }

        public int CandidateId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public string ExtractedText { get; set; } = string.Empty;

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;


        // Navigation properties

        public CandidateProfile Candidate { get; set; } = null!;

        public AIAnalysis? AIAnalysis { get; set; }

        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}