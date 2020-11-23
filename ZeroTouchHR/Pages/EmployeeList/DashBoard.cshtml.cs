using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ZeroTouchHR.Models;

namespace ZeroTouchHR.Pages.EmployeeList
{
    [Authorize(Roles = "Admin")]
    public class DashBoardModel : PageModel
    {

        private readonly ILogger<DashBoardModel> _logger;
        private readonly ApplicationDbContext _db;

        public DashBoardModel(ILogger<DashBoardModel> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public Employee employee { get; set; }
        public void OnGet()
        {

            ViewData["StartedCount"] = _db.employee.Where(e => e.Status == "Started").Count();
            ViewData["InprocessCount"] = _db.employee.Where(e => e.Status == "Inprocess").Count();
            ViewData["CompletedCount"] = _db.employee.Where(e => e.Status == "Completed").Count();

        }




    }
}
