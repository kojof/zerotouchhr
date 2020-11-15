using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ZeroTouchHR.Models;
using ZeroTouchHR.Pages.Account;

namespace ZeroTouchHR.Pages.EmployeeList
{

    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {

        private readonly ApplicationDbContext _db;
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly CognitoUserManager<CognitoUser> _userManager;
        private readonly ILogger<CreateModel> _logger;
        private readonly CognitoUserPool _pool;

        public CreateModel(ApplicationDbContext db, UserManager<CognitoUser> userManager, SignInManager<CognitoUser> signInManager, ILogger<CreateModel> logger, CognitoUserPool pool)
        {
            _db = db;
            _userManager = userManager as CognitoUserManager<CognitoUser>;
            _signInManager = signInManager;
            _logger = logger;
            _pool = pool;
        }

        [BindProperty]
        public Employee Employee { get; set; }

        public string ReturnUrl { get; set; }

        public void OnGet()
        {
        }


        //public async Task<IActionResult> OnPost()
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Employee.Status = "Started";
        //        await _db.employee.AddAsync(Employee);
        //        await _db.SaveChangesAsync();
        //        return RedirectToPage("Index");
        //    }
        //    else
        //    {

        //        return Page();
        //    }

        //}

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {

                await SaveToDatabase();

                var user = _pool.GetUser(Employee.Email);
                user.Attributes.Add(CognitoAttribute.Email.AttributeName, Employee.Email);

                var result = await _userManager.CreateAsync(user, Employee.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                   // await _signInManager.SignInAsync(user, isPersistent: false);

                    //return RedirectToPage("./Account/ConfirmAccount");
                    return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }


        private async Task SaveToDatabase()
        {
            Employee.Status = "Started";
            await _db.employee.AddAsync(Employee);
            await _db.SaveChangesAsync();
        }
    }
}
