using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ZeroTouchHR.Pages.User
{
    [AllowAnonymous]
    public class ConfirmAccountModel : PageModel
    {
        private readonly CognitoUserManager<CognitoUser> _userManager;
        private readonly SignInManager<CognitoUser> _signInManager;

        public ConfirmAccountModel(UserManager<CognitoUser> userManager, SignInManager<CognitoUser> signInManager)
        {
            _userManager = userManager as CognitoUserManager<CognitoUser>;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }


        public class InputModel
        {
            [Required]
            [Display(Name = "Code")]
            public string Code { get; set; }
            [Display(Name = "UserName")]
            public string UserName { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
          
            if (ModelState.IsValid)
            {
               // var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
               var userId = Input.UserName;
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{userId}'.");
                }

                var result = await _userManager.ConfirmSignUpAsync(user, Input.Code, true);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Error confirming account for user with ID '{userId}':");
                }
                else
                {
                    var response = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                    return RedirectToPage("./Index", new {emailAddress = userId });
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
