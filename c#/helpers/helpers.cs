using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;

namespace HelperUtil
{
    public static class helpers
    {
  
    private string GetRandomString() {
      var random = new Random();
      return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[random.Next(s.Length)]).ToArray());
    }//end GetRandomString()
   
    public static void sendEmail(CustomForm cform)
        {
            try
            {
                string username, pw, _from;
                StringBuilder _body = new StringBuilder();
             
                username = ConfigurationManager.AppSettings["MailUser"].ToString();
                pw = ConfigurationManager.AppSettings["MailPW"].ToString();
                _from = "noreply@randomurl.com";
                
                SmtpElement smtpSettings = Config.Get<SystemConfig>().SmtpSettings;
                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPHost"].ToString(), Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"].ToString()));
                smtpClient.Credentials = new NetworkCredential(username, pw);
                MailMessage mail = new MailMessage(_from, "randomuser@test.com");
                _body.Append("Topic: " + cform.Topic);
                _body.AppendLine();
                _body.AppendLine();
                _body.Append("First Name: " + cform.FirstName);
                _body.AppendLine();
                _body.AppendLine();
                _body.Append("Last Name: " + cform.LastName);
                _body.AppendLine();
                _body.AppendLine();
                _body.Append("Company: " + cform.Company);
                _body.AppendLine();
                _body.AppendLine();
                _body.Append("City: " + cform.City);
                _body.AppendLine();
                _body.AppendLine();
                _body.Append("State/Province: " + cform.State);
                _body.AppendLine();
                _body.AppendLine();
                _body.Append("Zip/Postal Code: " + cform.Zip);
                _body.AppendLine();
                _body.AppendLine();
                _body.Append("Country: " + cform.Country);
                _body.AppendLine();
                _body.AppendLine();
                _body.Append("Phone Number: " + cform.Phone);
                _body.AppendLine();
                _body.AppendLine();
                _body.Append("Email: " + cform.Email);
                _body.AppendLine();
                _body.AppendLine();
                _body.Append("Question: " + cform.Question);
                _body.AppendLine();
                _body.AppendLine();
                _body.AppendLine();
                _body.AppendLine();



                mail.Subject = "Contact Form Contents";
                mail.Body = _body.ToString();

                var message = mail;

                smtpClient.Send(message);
            }
            catch (Exception e)
            {
              //log
            }
        }


    }
}