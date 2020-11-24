using Microsoft.EntityFrameworkCore;
using ZeroTouchHR.Models;

namespace ZeroTouchHR.EmployeeBenefits.WorkerService.DataModels
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options) : base (options)
        {


        }

        public DbSet<Employee> employee { get; set; }
    }
}
