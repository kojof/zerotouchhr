using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ZeroTouchHR.Models;

namespace ZeroTouchHR.Pages.EmployeeList
{
    [Authorize]
    public class IndexModel : PageModel
    {

        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Employee> employees { get; set; }
        public IEnumerable<Employee> employeesCompleted { get; set; }

        public async Task OnGet()
        {
            employees = await _db.employee.Where(e => e.Status == "Started").ToListAsync();
            employeesCompleted = await _db.employee.Where(e => e.Status == "Completed").ToListAsync();

        }
    }
}
