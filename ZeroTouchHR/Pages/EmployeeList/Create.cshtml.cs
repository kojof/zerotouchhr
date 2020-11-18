using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ZeroTouchHR.Domain.Entities;
using ZeroTouchHR.Models;
using ZeroTouchHR.Pages.Account;
using ZeroTouchHR.Services;
using ZeroTouchHR.Services.Interfaces;

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
        private readonly ISQSService _sQSService;
      //  private readonly IConfiguration _configuration;
        //private IConfiguration _configuration;

        public CreateModel(ApplicationDbContext db, UserManager<CognitoUser> userManager, SignInManager<CognitoUser> signInManager, ILogger<CreateModel> logger, CognitoUserPool pool, ISQSService sQSService)
        {
            _db = db;
            _userManager = userManager as CognitoUserManager<CognitoUser>;
            _signInManager = signInManager;
            _logger = logger;
            _pool = pool;
            _sQSService = sQSService;
            // _configuration = configuration;
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

                  //  string message = "dsadd user cn=kishore,ou=users,ou=zerotouchhr,dc=zerotouchhr,dc=com -fn Kishore -ln Poosa -pwd $ervice1@3 -email kishore3886@gmail.com -memberof cn=WorkSpaces,ou=zerotouchhr,dc=zerotouchhr,dc=com";
                    
                  
                  var adUserCredentials = CreateAdUserCredentials();

                    var  messageSent = await _sQSService.SendMessageAsync(adUserCredentials);
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

        private ADUserCredentials CreateAdUserCredentials()
        {
            ADUserCredentials adUserCredentials = new ADUserCredentials
            {
                Email = Employee.Email,
                FirstName = Employee.FName,
                LastName = Employee.LName,
                Password = Employee.Password,
                UserName = Employee.UserName
            };
            return adUserCredentials;
        }


        private async Task SaveToDatabase()
        {
            Employee.Status = "Started";
            await _db.employee.AddAsync(Employee);
            await _db.SaveChangesAsync();
        }
    }
}
