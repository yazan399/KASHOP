using Microsoft.EntityFrameworkCore;
using System;
using KASHOP.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace KASHOP.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryTranslation> CategoryTranslations { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
      
    }
}
