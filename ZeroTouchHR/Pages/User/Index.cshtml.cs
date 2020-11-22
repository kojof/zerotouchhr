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

namespace ZeroTouchHR.Pages.User
{
    public class IndexModel : PageModel
    {

        private ApplicationDbContext _db;
        private readonly IHostingEnvironment hostingEnvironment;

        public IndexModel(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {

            _db = db;
            this.hostingEnvironment = hostingEnvironment;
        }



        //[BindProperty]
        //public Employee employee { get; set; }

        public class EmployeePhoto
        {
            [BindProperty]
            public Employee employee { get; set; }

            [System.ComponentModel.DataAnnotations.Schema.NotMapped]
            public IFormFile photo { get; set; }



        }


        public EmployeePhoto empPhoto { get; set; }

        public async Task OnGet(string email)
        {
             email = "kishore3886@gmail.com";
            //check it for user name
            // employee = _db.employee.Where(x => x.Email == email).FirstOrDefault();
            var emp = _db.employee.Where(x => x.Email == email).FirstOrDefault();

            empPhoto.employee = emp;

        }

        //this is to upload the document as well as push it to s3 bucket
        public async Task<IActionResult> OnPost()
        {

            if (ModelState.IsValid)
            {

                var EmpFromDb = await _db.employee.FindAsync(empPhoto.employee.id);

                //EmpFromDb.FName = employee.FName;
                //EmpFromDb.LName = employee.LName;
                //EmpFromDb.title = employee.title;
                //EmpFromDb.AddressLine1 = employee.AddressLine1;
                //EmpFromDb.AddressLine2 = employee.AddressLine2;
                //EmpFromDb.PhoneNumber = employee.PhoneNumber;
                //EmpFromDb.State = employee.State;
                //EmpFromDb.City = employee.City;
                //EmpFromDb.Zip = employee.Zip;
                //EmpFromDb.Email = employee.Email;
                //EmpFromDb.Password = employee.Password;
                EmpFromDb.Status = "User Verified";
                //employee.Status = "Started";
                //await _db.employee.AddAsync(employee);
                //await _db.SaveChangesAsync();
                //return RedirectToPage("Index");
               // await _db.SaveChangesAsync();
                //Push the file to s3 using awssdk
                UpLoadTOs3.PushToS3();
                return RedirectToPage("Thankyou");
            }

            return RedirectToPage();

        }


        string filePath = "";
        public async Task<IActionResult> upload(EmployeePhoto model)
        {

            if(ModelState.IsValid)
            {
                if(model.photo!=null)
                {
                    //to get physical path use IHostingEnvironment
                    //we can set file name as username here
                    string uploadsFolder= Path.Combine(hostingEnvironment.WebRootPath, "images");
                    string uniqueName = model.employee.UserName;
                    filePath = Path.Combine(uploadsFolder, uniqueName);
                    model.photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                
                Employee employee = new Employee
                {

                    //write if any field has to be changed - Probably status
                };

                UpLoadTOs3.PushToS3(filePath);

            }



            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostfileUpload(IFormFile file)
        {


            return RedirectToPage("Thankyou");

        }



    }

    //public class EmployeePhoto
    //{
    //    [BindProperty]
    //    public Employee employee { get; set; }

    //    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    //    IFormFile photo { get; set; }



    //}

    class UpLoadTOs3 {

        private const string bucketName = "zerotouchhr/users";
        private const string keyName = "TempUserFIle";
        private const string filePath = @"C:\Users\kisho\Downloads";
        // Specify your bucket region (an example region is shown).
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USWest2;
        private static IAmazonS3 s3Client;


        public static void PushToS3(string filePath)
        {
            s3Client = new AmazonS3Client(bucketRegion);
            UploadFileAsync().Wait();
        }

        private static async Task UploadFileAsync()
        {
            try
            {
                var fileTransferUtility =
                    new TransferUtility(s3Client);

                // Option 1. Upload a file. The file name is used as the object key name.
                await fileTransferUtility.UploadAsync(filePath, bucketName);
                Console.WriteLine("Upload 1 completed");

                //// Option 2. Specify object key name explicitly.
                //await fileTransferUtility.UploadAsync(filePath, bucketName, keyName);
                //Console.WriteLine("Upload 2 completed");

                //// Option 3. Upload data from a type of System.IO.Stream.
                //using (var fileToUpload =
                //    new FileStream(filePath, FileMode.Open, FileAccess.Read))
                //{
                //    await fileTransferUtility.UploadAsync(fileToUpload,
                //                               bucketName, keyName);
                //}
                //Console.WriteLine("Upload 3 completed");

                //// Option 4. Specify advanced settings.
                //var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                //{
                //    BucketName = bucketName,
                //    FilePath = filePath,
                //    StorageClass = S3StorageClass.StandardInfrequentAccess,
                //    PartSize = 6291456, // 6 MB.
                //    Key = keyName,
                //    CannedACL = S3CannedACL.PublicRead
                //};
                //fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                //fileTransferUtilityRequest.Metadata.Add("param2", "Value2");

                //await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
                //Console.WriteLine("Upload 4 completed");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }

        }


        //public async Task CreateFoldersAsync(string bucketName, string path)
        //{
        //   // path = path.EnsureEndsWith('/');

        //    //need to pass our access key and secret key
        //    IAmazonS3 client =
        //        new AmazonS3Client(AccessKeyId,SecretAccessKey,
        //        RegionEndpoint.EUWest1);

        //    var findFolderRequest = new ListObjectsV2Request();
        //    findFolderRequest.BucketName = bucketName;
        //    findFolderRequest.Prefix = path;

        //    ListObjectsV2Response findFolderResponse =
        //       await client.ListObjectsV2Async(findFolderRequest);


        //    if (findFolderResponse.S3Objects.Any())
        //    {
        //        return;
        //    }

        //    PutObjectRequest request = new PutObjectRequest()
        //    {
        //        BucketName = bucketName,
        //        StorageClass = S3StorageClass.Standard,
        //        ServerSideEncryptionMethod = ServerSideEncryptionMethod.None,
        //        Key = path,
        //        ContentBody = string.Empty
        //    };

        //    // add try catch in case you have exceptions shield/handling here 
        //    PutObjectResponse response = await client.PutObjectAsync(request);
        //}

    }
}
