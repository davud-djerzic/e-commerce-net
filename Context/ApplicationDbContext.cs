using Microsoft.EntityFrameworkCore;
using E_commerce_API.Models;

namespace E_commerce_API.Context 
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categorys { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
