using Microsoft.EntityFrameworkCore;

namespace ZeroTouchHR.Models
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options) : base (options)
        {


        }

        public DbSet<Employee> Employee { get; set; }
    }
}
