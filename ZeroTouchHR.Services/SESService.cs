using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using ZeroTouchHR.Services.Interfaces;

namespace ZeroTouchHR.Services
{

    public class SESService : ISESService
    {
        // Replace sender@example.com with your "From" address.
        // This address must be verified with Amazon SES.
        static readonly string senderAddress = "kojof@hotmail.com";

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
          <p>hi John,</p>
          <p>Please see a list of benefits that you will be receiving as a new employee.</p>
          <p>regards</p>
          <p>HR Admnistrator</p>           
        </body>
        </html>";

        public SESService()
        {

        }

        public async Task Send(string emailAddress)
        {
           
            using (var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.USEast1))
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = senderAddress,
                    Destination = new Destination
                    {
                        ToAddresses =
                        new List<string> { emailAddress }
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
                            },
                            //Text = new Content
                            //{
                            //    Charset = "UTF-8",
                            //    Data = textBody
                            //}
                        }
                    },
                    // If you are not using a configuration set, comment
                    // or remove the following line 
                    //ConfigurationSetName = configSet
                };
                try
                {
                    Console.WriteLine("Sending email using Amazon SES...");
                    var response = await client.SendEmailAsync(sendRequest);
                    Console.WriteLine("The email was sent successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The email was not sent.");
                    Console.WriteLine("Error message: " + ex.Message);

                }
            }

            //    Console.Write("Press any key to continue...");
            //   Console.ReadKey();
        }
    }
}
