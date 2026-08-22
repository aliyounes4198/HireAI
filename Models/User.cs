using System.ComponentModel.DataAnnotations;

namespace HireAI.API.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Candidate, HR, Admin
        [Required]
        public string Role { get; set; } = "Candidate";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key to Company
        public int? CompanyId { get; set; }

        // Navigation properties
        public Company? Company { get; set; }

        public CandidateProfile? CandidateProfile { get; set; }
    }
}