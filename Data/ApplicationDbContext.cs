
using HireAI.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HireAI.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Database tables

        public DbSet<User> Users { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<CandidateProfile> CandidateProfiles { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<Resume> Resumes { get; set; }

        public DbSet<Application> Applications { get; set; }

        public DbSet<AIAnalysis> AIAnalyses { get; set; }

        public DbSet<JobMatch> JobMatches { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // User - Company relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);


            // User - CandidateProfile relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.CandidateProfile)
                .WithOne(c => c.User)
                .HasForeignKey<CandidateProfile>(c => c.UserId);


            // Company - Job relationship
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Company)
                .WithMany(c => c.Jobs)
                .HasForeignKey(j => j.CompanyId);


            // CandidateProfile - Resume relationship
            modelBuilder.Entity<Resume>()
                .HasOne(r => r.Candidate)
                .WithMany(c => c.Resumes)
                .HasForeignKey(r => r.CandidateId);


            // Resume - AIAnalysis relationship
            modelBuilder.Entity<AIAnalysis>()
                .HasOne(a => a.Resume)
                .WithOne(r => r.AIAnalysis)
                .HasForeignKey<AIAnalysis>(a => a.ResumeId);


            // Job - Application relationship
            modelBuilder.Entity<Application>()
                .HasOne(a => a.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobId);


            // Candidate - Application relationship
            modelBuilder.Entity<Application>()
                .HasOne(a => a.Candidate)
                .WithMany()
                .HasForeignKey(a => a.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);


            // Resume - Application relationship
            modelBuilder.Entity<Application>()
                .HasOne(a => a.Resume)
                .WithMany(r => r.Applications)
                .HasForeignKey(a => a.ResumeId)
                .OnDelete(DeleteBehavior.Restrict);


            // Resume - JobMatch relationship
            modelBuilder.Entity<JobMatch>()
                .HasOne(m => m.Resume)
                .WithMany()
                .HasForeignKey(m => m.ResumeId)
                .OnDelete(DeleteBehavior.Restrict);


            // Job - JobMatch relationship
            modelBuilder.Entity<JobMatch>()
                .HasOne(m => m.Job)
                .WithMany()
                .HasForeignKey(m => m.JobId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

