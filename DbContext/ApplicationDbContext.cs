﻿using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.DbContext
{
    public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Service> Services { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<SubPackage> SubPackages { get; set; }

    }
}