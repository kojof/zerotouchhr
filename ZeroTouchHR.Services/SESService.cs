using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ZeroTouchHR.Services.Interfaces;

namespace ZeroTouchHR.Services
{

    public class SESService : ISESService
    {
        // Replace sender@example.com with your "From" address.
        // This address must be verified with Amazon SES.
     //   static readonly string senderAddress = "kojof@hotmail.com";

        // Replace recipient@example.com with a "To" address. If your account
        // is still in the sandbox, this address must be verified.
        //static readonly string receiverAddress = "kojof@calabashmedia.com";

        // The configuration set to use for this email. If you do not want to use a
        // configuration set, comment out the following property and the
        // ConfigurationSetName = configSet argument below. 
        static readonly string configSet = "ConfigSet";

        // The subject line for the email.
        static readonly string subject = "Welcome to Zero Touch HR";

        // The email body for recipients with non-HTML email clients.
        static readonly string textBody = "Amazon SES Test (.NET)\r\n"
                                          + "This email was sent through Amazon SES "
                                          + "using the AWS SDK for .NET.";

        // The HTML body of the email.
        //        static readonly string htmlBody = @"<html>
        //<head></head>
        //<body>
        //  <h1>Zero Touch HR Registration</h1>
        //  <p>Please login to Zero Touch HR website to complete registration  
        //    <a href='https://dev.zerotouchhr.com/Account/Register/'> registration   </a>.</p>
        //</body>
        //</html>";

        static readonly string htmlBody = @"<html>
        <head></head>
        <body>
          <h1>Welcome to Zero Touch HR</h1>
          <p>hi,</p>
          <p>Please see a list of benefits that you will be receiving as a new employee.</p>
            <p>Please contact HR Team on this number - +1 216 539 2077 for any queries.</p>
          <p>regards</p>
          <p>HR Admnistrator</p>           
        </body>
        </html>";

        private readonly ILogger<SESService> _logger;
        private readonly IAmazonSimpleEmailService _amazonSimpleEmailService;
        private readonly IConfiguration _configuration;


        public SESService(IAmazonSimpleEmailService amazonSimpleEmailService, IConfiguration configuration, ILogger<SESService> logger)
        {
            _logger = logger;
            _amazonSimpleEmailService = amazonSimpleEmailService;
            _configuration = configuration;
        }

        public async Task<HttpStatusCode> SendEmailAsync(string emailAddress)
        {
            try
            {
                string senderEmailAddress = _configuration.GetSection("AWS").GetSection("SESEmailSender").Value;
                _logger.LogInformation($"Entered SES Service SendEmailAsync Method on {DateTime.UtcNow:O}");
                _logger.LogInformation($"SES Service SendEmailAsync Method show Sender EmailAddress from Config - {senderEmailAddress}   {DateTime.UtcNow:O}");

                var sendRequest = new SendEmailRequest
                {
                    Source = senderEmailAddress,
                    Destination = new Destination
                    {
                        ToAddresses =
                            new List<string> {emailAddress}
                    },
                    Message = new Message
                    {
                        Subject = new Content(subject),
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = "UTF-8",
                                Data = htmlBody
                            }
                        }
                    }
                };


                var response = await _amazonSimpleEmailService.SendEmailAsync(sendRequest);
                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation($"The email with message Id {response.MessageId} sent successfully to {sendRequest.Destination.ToAddresses} on {DateTime.UtcNow:O}");
                }
                else
                {
                    _logger.LogError(
                        $"Failed to send email with message Id {response.MessageId} to {sendRequest.Destination.ToAddresses} on {DateTime.UtcNow:O} due to {response.HttpStatusCode}.");
                }

                return response.HttpStatusCode;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email  on {DateTime.UtcNow:O} due to {ex.InnerException}.");
                _logger.LogError($"Failed to send email  on {DateTime.UtcNow:O} due to {ex.StackTrace}.");

            }

            return HttpStatusCode.BadRequest;
        }
    }
}

