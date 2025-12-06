using Microsoft.EntityFrameworkCore;
using Sensore.Models;

namespace Sensore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<PressureFrame> PressureFrames => Set<PressureFrame>();
        public DbSet<Metric> Metrics => Set<Metric>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<AlertLog> AlertLogs => Set<AlertLog>();
        public DbSet<ClinicianPatient> ClinicianPatients => Set<ClinicianPatient>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<PressureFrame>()
                .HasKey(p => p.FrameId);

            modelBuilder.Entity<PressureFrame>()
                .HasOne(p => p.User)
                .WithMany(u => u.PressureFrames)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasKey(c => c.CommentId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AlertLog>()
                .HasKey(a => a.AlertId);

            modelBuilder.Entity<ClinicianPatient>()
                .HasKey(cp => cp.Id);

            modelBuilder.Entity<ClinicianPatient>()
                .HasIndex(cp => new { cp.ClinicianId, cp.PatientId })
                .IsUnique();
        }
    }
}
