using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.DbContext
{
    public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<ShopService> ShopServices { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<SubPackage> SubPackages { get; set; }
        public DbSet<AdminStaticResource> AdminStaticResources { get; set; }
        public DbSet<FeedBack> FeedBacks { get; set; }
        public DbSet<ShopServiceStaticResources> ShopServiceStaticResources { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ShopService>()
                .HasOne(x => x.Shop)
                .WithMany(x => x.Services)
                .HasForeignKey(x => x.ShopId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ShopService>()
                .HasOne(x => x.Category)
                .WithMany(x => x.ShopServices)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShopServiceStaticResources>()
                .HasOne(x => x.Service)
                .WithMany(x => x.ShopServiceStaticResources)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubPackage>()
                .HasOne(x => x.Package)
                .WithMany(x => x.SubPackages)
                .HasForeignKey(x => x.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FeedBack>()
                .HasOne(x => x.Service)
                .WithMany(x => x.FeedBacks)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShopServiceStaticResources>()
                .HasOne(x => x.Service)
                .WithMany(x => x.ShopServiceStaticResources)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
