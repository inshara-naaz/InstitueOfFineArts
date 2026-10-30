using InstituteOfFineArts.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InstituteOfFineArts.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Staff> Staff { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<CompetitionAward> CompetitionAwards { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Award> Awards { get; set; }
        public DbSet<Exhibition> Exhibitions { get; set; }
        public DbSet<ExhibitionListing> ExhibitionListings { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ── Staff ──────────────────────────────────────────────────────────
            builder.Entity<Staff>()
                .HasOne(s => s.ApplicationUser)
                .WithOne(u => u.Staff)
                .HasForeignKey<Staff>(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Student ────────────────────────────────────────────────────────
            builder.Entity<Student>()
                .HasOne(s => s.ApplicationUser)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Student>()
                .HasIndex(s => s.EnrollmentNumber)
                .IsUnique();

            // ── Competition ────────────────────────────────────────────────────
            builder.Entity<Competition>()
                .HasOne(c => c.ManagedByStaff)
                .WithMany(s => s.ManagedCompetitions)
                .HasForeignKey(c => c.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── CompetitionAward ───────────────────────────────────────────────
            builder.Entity<CompetitionAward>()
                .HasOne(ca => ca.Competition)
                .WithMany(c => c.CompetitionAwards)
                .HasForeignKey(ca => ca.CompetitionId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Submission ─────────────────────────────────────────────────────
            builder.Entity<Submission>()
                .HasOne(s => s.Student)
                .WithMany(st => st.Submissions)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Submission>()
                .HasOne(s => s.Competition)
                .WithMany(c => c.Submissions)
                .HasForeignKey(s => s.CompetitionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Submission>()
                .HasIndex(s => new { s.StudentId, s.CompetitionId })
                .IsUnique();

            // ── Evaluation ─────────────────────────────────────────────────────
            builder.Entity<Evaluation>()
                .HasOne(e => e.Submission)
                .WithOne(s => s.Evaluation)
                .HasForeignKey<Evaluation>(e => e.SubmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Evaluation>()
                .HasOne(e => e.EvaluatedByStaff)
                .WithMany(s => s.Evaluations)
                .HasForeignKey(e => e.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Award ──────────────────────────────────────────────────────────
            builder.Entity<Award>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Awards)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Award>()
                .HasOne(a => a.Competition)
                .WithMany(c => c.IssuedAwards)
                .HasForeignKey(a => a.CompetitionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Award>()
                .HasOne(a => a.CompetitionAward)
                .WithMany(ca => ca.IssuedAwards)
                .HasForeignKey(a => a.CompetitionAwardId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Award>()
                .HasOne(a => a.WinningSubmission)
                .WithMany()
                .HasForeignKey(a => a.SubmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Award>()
                .HasOne(a => a.IssuedByStaff)
                .WithMany(s => s.IssuedAwards)
                .HasForeignKey(a => a.IssuedByStaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Exhibition ─────────────────────────────────────────────────────
            builder.Entity<Exhibition>()
                .HasOne(e => e.ManagedByStaff)
                .WithMany(s => s.ManagedExhibitions)
                .HasForeignKey(e => e.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── ExhibitionListing ──────────────────────────────────────────────
            builder.Entity<ExhibitionListing>()
                .HasOne(el => el.Exhibition)
                .WithMany(e => e.ExhibitionListings)
                .HasForeignKey(el => el.ExhibitionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExhibitionListing>()
                .HasOne(el => el.Submission)
                .WithMany(s => s.ExhibitionListings)
                .HasForeignKey(el => el.SubmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExhibitionListing>()
                .HasOne(el => el.ForwardedByStaff)
                .WithMany(s => s.ExhibitionListings)
                .HasForeignKey(el => el.ForwardedByStaffId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExhibitionListing>()
                .HasIndex(el => new { el.ExhibitionId, el.SubmissionId })
                .IsUnique();
        }
    }
}