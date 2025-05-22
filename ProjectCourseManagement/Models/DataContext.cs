using Microsoft.EntityFrameworkCore;
using ProjectCourseManagement.Areas.Admin.Models;

namespace ProjectCourseManagement.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<AdminMenu> AdminMenus { get; set; }

        public DbSet<Users> User { get; set; }

        public DbSet<Course> Course { get; set; }
    }
}
