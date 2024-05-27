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
        public DbSet<FeedBackStaticResource> FeedBackStaticResources { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShopService>()
                .HasOne(x => x.Shop)
                .WithMany(x => x.Services)
                .HasForeignKey(x => x.ShopId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<ShopService>()
                .HasOne(x => x.Category)
                .WithMany(x => x.ShopServices)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ShopServiceStaticResources>()
                .HasOne(x => x.Service)
                .WithMany(x => x.ShopServiceStaticResources)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Package>()
                .HasOne(x => x.User)
                .WithMany(x => x.Packages)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SubPackage>()
                .HasOne(x => x.Package)
                .WithMany(x => x.SubPackages)
                .HasForeignKey(x => x.PackageId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SubPackage>()
                .HasOne(x => x.Service)
                .WithMany(x => x.SubPackages)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FeedBack>()
                .HasOne(x => x.Service)
                .WithMany(x => x.FeedBacks)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ShopServiceStaticResources>()
                .HasOne(x => x.Service)
                .WithMany(x => x.ShopServiceStaticResources)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FeedBackStaticResource>()
                .HasOne(x => x.FeedBack)
                .WithMany(x => x.FeedBackStaticResources)
                .HasForeignKey(x => x.FeedBackId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AdminStaticResource>()
                .HasOne(x => x.Admin)
                .WithMany(x => x.AdminStaticResources)
                .HasForeignKey(x => x.AdminId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(x => x.Sender)
                .WithMany(x => x.SentMessages)
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(x => x.Receiver)
                .WithMany(x => x.RecievedMessages)
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Shop>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.Shops)
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Category>()
                .HasOne(x => x.Admin)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.AdminId)
                .OnDelete(DeleteBehavior.NoAction);

        }

    }
}
