using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookWormHub.Models;

namespace BookWormHub.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        }

        // Register tables
        public DbSet<Book> Books { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<BannedWord> BannedWords { get; set; }

        // Config
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ISBN-13 unique
            builder.Entity<Book>()
                .HasIndex(b => b.ISBN13)
                .IsUnique();

            // Book property constraints (replaces Data Annotations)
            builder.Entity<Book>(entity =>
            {
                entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
                entity.Property(b => b.Author).IsRequired().HasMaxLength(100);
                entity.Property(b => b.ISBN13).IsRequired().HasMaxLength(13);
                entity.Property(b => b.Genre).HasMaxLength(50);
                entity.Property(b => b.Description).HasMaxLength(2000);
            });

            // Review property constraints
            builder.Entity<Review>(entity =>
            {
                entity.Property(r => r.UserId).IsRequired();
                entity.Property(r => r.Comment).HasMaxLength(2000);
            });

            // BannedWord property constraints
            builder.Entity<BannedWord>(entity =>
            {
                entity.Property(bw => bw.Word).IsRequired().HasMaxLength(100);
            });

            // 1 user chỉ 1 review per book (composite unique index)
            builder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.BookId })
                .IsUnique();

            // 1 User - N Reviews
            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)       // ← UserId, KHÔNG PHẢI User
                .OnDelete(DeleteBehavior.Cascade);

            // 1 Book - N Reviews
            builder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Badword must unique
            builder.Entity<BannedWord>()
                .HasIndex(bw => bw.Word)
                .IsUnique();
        }
    }
}
