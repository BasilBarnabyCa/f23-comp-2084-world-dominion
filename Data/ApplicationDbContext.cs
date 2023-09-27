using Microsoft.EntityFrameworkCore;

namespace WorldDominion.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        public DbSet<Department> Departments { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}