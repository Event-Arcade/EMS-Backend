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

            modelBuilder.Entity<Package>()
                .HasOne(x => x.User)
                .WithMany(x => x.Packages)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubPackage>()
                .HasOne(x => x.Package)
                .WithMany(x => x.SubPackages)
                .HasForeignKey(x => x.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubPackage>()
                .HasOne(x => x.Service)
                .WithMany(x => x.SubPackages)
                .HasForeignKey(x => x.ServiceId)
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

            modelBuilder.Entity<FeedBackStaticResource>()
                .HasOne(x => x.FeedBack)
                .WithMany(x => x.FeedBackStaticResources)
                .HasForeignKey(x => x.FeedBackId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdminStaticResource>()
                .HasOne(x => x.Admin)
                .WithMany(x => x.AdminStaticResources)
                .HasForeignKey(x => x.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(x => x.Sender)
                .WithMany(x => x.SentMessages)
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(x => x.Receiver)
                .WithMany(x => x.RecievedMessages)
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);


            // Data Seeding for the Category Table
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Automotive", AdminId = "2908e25a-b961-4dea-85b2-9ec84c1e6226", CategoryImagePath = "https://via.placeholder.com/150" },
                new Category { Id = 2, Name = "Beauty", AdminId = "2908e25a-b961-4dea-85b2-9ec84c1e6226", CategoryImagePath = "https://via.placeholder.com/150" },
                new Category { Id = 3, Name = "Construction", AdminId = "2908e25a-b961-4dea-85b2-9ec84c1e6226", CategoryImagePath = "https://via.placeholder.com/150" },
                new Category { Id = 4, Name = "Education", AdminId = "2908e25a-b961-4dea-85b2-9ec84c1e6226", CategoryImagePath = "https://via.placeholder.com/150" },
                new Category { Id = 5, Name = "Entertainment", AdminId = "2908e25a-b961-4dea-85b2-9ec84c1e6226", CategoryImagePath = "https://via.placeholder.com/150" },
                new Category { Id = 6, Name = "Food", AdminId = "2908e25a-b961-4dea-85b2-9ec84c1e6226", CategoryImagePath = "https://via.placeholder.com/150" },
                new Category { Id = 7, Name = "Health", AdminId = "2908e25a-b961-4dea-85b2-9ec84c1e6226", CategoryImagePath = "https://via.placeholder.com/150" },
                new Category { Id = 8, Name = "IT", AdminId = "2908e25a-b961-4dea-85b2-9ec84c1e6226", CategoryImagePath = "https://via.placeholder.com/150" },
                new Category { Id = 9, Name = "Legal", AdminId = "2908e25a-b961-4dea-85b2-9ec84c1e6226", CategoryImagePath = "https://via.placeholder.com/150" },
                new Category { Id = 10, Name = "Manufacturing", AdminId = "2908e25a-b961-4dea-85b2-9ec84c1e6226", CategoryImagePath = "https://via.placeholder.com/150" },
                new Category { Id = 11, Name = "Retail", AdminId = "2908e25a-b961-4dea-85b2-9ec84c1e6226", CategoryImagePath = "https://via.placeholder.com/150" },
                new Category { Id = 12, Name = "Services", AdminId = "2908e25a-b961-4dea-85b2-9ec84c1e6226", CategoryImagePath = "https://via.placeholder.com/150" },
                new Category { Id = 13, Name = "Transportation", AdminId = "2908e25a-b961-4dea-85b2-9ec84c1e6226", CategoryImagePath = "https://via.placeholder.com/150" }
            );

            // Data Seeding for the Shop Table
            modelBuilder.Entity<Shop>().HasData(
                 new Shop { Id = 1, Name = "Shop 1", OwnerId = "9b982dc2-f99d-4c9b-b3db-c6ed2e193c98", BackgroundImagePath = "https://via.placeholder.com/150" }
                
            );

            // Data Seeding for the ShopService Table
            modelBuilder.Entity<ShopService>().HasData(
                new ShopService { Id = 1, Name = "Service 1", Price = 100, Description = "Service 1 Description", Rating = 4.5, ShopId = 1, CategoryId = 1 }
            

            );

        }

    }
}
