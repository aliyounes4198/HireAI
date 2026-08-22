namespace HireAI.API.Models
{
    public class Company
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Website { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // HR users
        public ICollection<User> Users { get; set; } = new List<User>();

        // Jobs posted by company
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
