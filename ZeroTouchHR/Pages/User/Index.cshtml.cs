using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZeroTouchHR.Models;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.IO;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.AspNetCore.Hosting;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using ZeroTouchHR.Services.Interfaces;

namespace ZeroTouchHR.Pages.User
{
    public class IndexModel : PageModel
    {

        private ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostingEnvironment;
        private const string BUCKET_NAME = "zerotouchhr";
        private readonly IS3Service _s3Service;

        public IndexModel(ApplicationDbContext db, IWebHostEnvironment webHostingEnvironment, IS3Service s3Service)
        {

            _db = db;
            _webHostingEnvironment = webHostingEnvironment;
            _s3Service = s3Service;
        }

        [BindProperty] public Employee Employee { get; set; }

        public async Task UploadFileAsync(string emailAddress)
        {
            string folderName = emailAddress;
            var folderPath = Path.Combine(_webHostingEnvironment.WebRootPath, "Files");

            //hard-code this for now, as we don't use the uploaded document 
            string folderFilePath = Path.Combine(folderPath, "documentverification.txt");

            using (FileStream fsSource = new FileStream(folderFilePath, FileMode.Open, FileAccess.Read))
            {
                string fileExtension = Path.GetExtension(folderFilePath);
                //string fileName = string.Empty;
                string fileName = $"{Guid.NewGuid().ToString()}{fileExtension}";
                await _s3Service.UploadFile(fsSource, fileName, folderName);
            }
        }


        public async Task OnGetAsync(string emailAddress)
        {
            Employee = _db.employee.FirstOrDefault(x => x.Email == emailAddress);
        }

        //this is to upload the document as well as push it to s3 bucket
        public async Task<IActionResult> OnPostAsync()
        {

            if (ModelState.IsValid)
            {

                var email = Employee.Email;

                //create a file and folder in bucket
                await UploadFileAsync(email);
                

                return RedirectToPage("Thankyou");
            }

            return RedirectToPage();
        }
    }
}
