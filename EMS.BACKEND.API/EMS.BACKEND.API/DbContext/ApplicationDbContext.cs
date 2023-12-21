using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EMS.BACKEND.API.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<Vendor>
    {
        public ApplicationDbContext(DbContextOptions options): base(options) { }
    }
}
