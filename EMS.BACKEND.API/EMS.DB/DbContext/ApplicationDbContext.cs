using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.EntityFrameworkCore;
using EMS.DB.Models;


namespace EMS.DB.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<Vender>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

    }
}
