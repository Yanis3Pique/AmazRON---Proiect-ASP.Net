using AmazRON.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AmazRON.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<UserProduct> UserProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // definire relatii cu modelele Product si User (FK)
            // pentru UserProduct
            modelBuilder.Entity<UserProduct>()
                .HasKey(up => new { up.Id, up.UserId, up.ProductId });
            modelBuilder.Entity<UserProduct>()
                .HasOne(ab => ab.Product)
                .WithMany(ab => ab.UserProducts)
                .HasForeignKey(ab => ab.ProductId);
            modelBuilder.Entity<UserProduct>()
                .HasOne(ab => ab.User)
                .WithMany(ab => ab.UserProducts)
                .HasForeignKey(ab => ab.UserId);

            // definire relatii cu modelele Product si User (FK)
            // pentru Review
            modelBuilder.Entity<Review>()
                .HasKey(up => new { up.Id, up.UserId, up.ProductId });
            modelBuilder.Entity<Review>()
                .HasOne(ab => ab.Product)
                .WithMany(ab => ab.Reviews)
                .HasForeignKey(ab => ab.ProductId);
            modelBuilder.Entity<Review>()
                .HasOne(ab => ab.User)
                .WithMany(ab => ab.Reviews)
                .HasForeignKey(ab => ab.UserId);
        }
    }
}