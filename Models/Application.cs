
namespace HireAI.API.Models
{
    public class Application
    {
        public int Id { get; set; }

        public int CandidateId { get; set; }

        public int JobId { get; set; }

        public int ResumeId { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;


        // Navigation properties
        // Nullable because they are loaded by Entity Framework,
        // not provided by the API request.

        public CandidateProfile? Candidate { get; set; }

        public Job? Job { get; set; }

        public Resume? Resume { get; set; }
    }
}

