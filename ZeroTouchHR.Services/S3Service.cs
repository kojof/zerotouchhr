using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ZeroTouchHR.Domain.Entities;
using ZeroTouchHR.Services.Interfaces;


namespace ZeroTouchHR.Services
{
    public class S3Service : IS3Service
    {

        private readonly IAmazonS3 _s3Client;

        //  private readonly string _sqsUrl;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IS3Service> _logger;

        public S3Service(ILogger<S3Service> logger, IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task UploadFile(System.IO.Stream inputStream, string fileName, string folderName)
        {
            try
            {
                string bucketName = _configuration.GetSection("AWS").GetSection("BucketName").Value;

                PutObjectRequest request = new PutObjectRequest()
                {
                    InputStream = inputStream,
                    BucketName  = bucketName,
                    Key = folderName + @"/" + fileName
                };
               
                PutObjectResponse response = await _s3Client.PutObjectAsync(request);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException.ToString());
            }
        }
    }

   
}