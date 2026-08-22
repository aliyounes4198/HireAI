
namespace HireAI.API.Models
{
    public class Job
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Requirements { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Company? Company { get; set; }

        public ICollection<Application> Applications { get; set; }
            = new List<Application>();
    }
}
