using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using AuthorizedServer.Logger;

namespace AuthorizedServer.Helper
{
    /// <summary>Helper methos for Amazon SES service for sending email</summary>
    public class EmailHelper
    {
        /// <summary>Get amazon SES credentials from xml file</summary>
        /// <param name="key"></param>
        public static string GetCredentials(string key)
        {
            try
            {
                var result = GlobalHelper.ReadXML().Elements("amazonses").Where(x => x.Element("current").Value.Equals("Yes")).Descendants(key);
                return result.First().Value;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("EmailHelper", "GetCredentials", ex.Message);
                return ex.Message;
            }
        }

        /// <summary>Send email using amason SES service</summary>
        /// <param name="fullname"></param>
        /// <param name="emailReceiver"></param>
        /// <param name="link"></param>
        public static async Task<string> SendEmail(string fullname,string emailReceiver, string link)
        {
            var cc = GetCredentials("accesskey");
            var ss = GetCredentials("secretkey");
            string emailSender = GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("emailsender").First().Value;
            using (var client = new AmazonSimpleEmailServiceClient(GetCredentials("accesskey"), GetCredentials("secretkey"), Amazon.RegionEndpoint.USWest2))
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = emailSender,
                    Destination = new Destination { ToAddresses = new List<string> { emailReceiver } },
                    Message = new Message
                    {
                        Subject = new Content(GlobalHelper.ReadXML().Elements("email").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("emailsubject2").First().Value),
                        Body = new Body
                        {
                            Html = new Content(CreateEmailBody(fullname, "<a href ='" + link + "' style='background-color:#2a2c2e; color:#fff; text-align:center; padding:10px 15px 7px; text-decoration:none;'>Click Here To Verify</a>"))
                        }
                    }
                };
                try
                {
                    var responce = await client.SendEmailAsync(sendRequest);
                    return "Success";
                }
                catch (Exception ex)
                {
                    LoggerDataAccess.CreateLog("EmailHelper", "SendEmail", ex.Message);
                    return ex.Message;
                }
            }
        }

        /// <summary>Create email body</summary>
        /// <param name="fullname"></param>
        /// <param name="link"></param>
        public static string CreateEmailBody(string fullname, string link)
        {
            try
            {
                string emailBody;
                var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var path = Path.Combine(dir, "EmailTemplate/VerificationEmail.html");
                using (StreamReader reader = File.OpenText(path))
                {
                    emailBody = reader.ReadToEnd();
                }
                emailBody = emailBody.Replace("{FullName}", fullname);
                emailBody = emailBody.Replace("{Link}", link);
                return emailBody;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("EmailHelper", "CreateEmailBody", ex.Message);
                return ex.Message;
            }
        }
    }
}
