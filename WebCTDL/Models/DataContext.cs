using Microsoft.EntityFrameworkCore;
using startup.Models;
namespace WebCTDL.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        { 
        }
        public DbSet<Menu> Menus { get; set; }

        public DbSet<Post> Posts { get; set; }
    }
}
