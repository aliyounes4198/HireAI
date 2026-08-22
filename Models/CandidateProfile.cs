namespace HireAI.API.Models
{
    public class CandidateProfile
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Phone { get; set; } = string.Empty;

        public string Skills { get; set; } = string.Empty;

        public string Education { get; set; } = string.Empty;

        public string Experience { get; set; } = string.Empty;

        public string LinkedIn { get; set; } = string.Empty;

        public string GitHub { get; set; } = string.Empty;


        // Navigation property

        [System.Text.Json.Serialization.JsonIgnore]
        public User User { get; set; } = null!;

        public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
    }
}