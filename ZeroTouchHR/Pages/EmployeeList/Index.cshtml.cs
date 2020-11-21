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
    [Authorize(Roles = "Admin")]
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

        public async Task<IActionResult> OnPostDelete(int id)
        {

            var emp = await _db.employee.FindAsync(id);

            if(emp==null)
            {

                return NotFound();
            }

            _db.employee.Remove(emp);
            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
