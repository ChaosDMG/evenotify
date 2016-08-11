using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ApplicationInsights;
using evenotify_v2.models;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace evenotify_v2.Controllers
{
    public class HomeController : Controller
    {
        private TelemetryClient telemetry = new TelemetryClient();

        public IActionResult Index(bool success = false, bool recapcha = false)
        {
            ViewBag.Title = "Evenotify";

            ViewBag.mask = false;
            ViewBag.character = false;
            ViewBag.invalid = false;
            ViewBag.exist = false;
            ViewBag.email = false;
            ViewBag.success = success;

            return View();

        }

        public async Task<IActionResult> mail(Guid id)
        {
            ViewBag.isValid = true;
            if (id == null)
            {
                ViewBag.isValid = false;
            }
            else
                try
                {
                    using (var db = new eve())
                    {

                        if (db.Users.Where(x => x.verifyUrl == id).First().verified != true)
                        {
                            db.Users.Where(x => x.verifyUrl == id).First().verified = true;
                            await db.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: '{0}'", e);
                    telemetry.TrackException(e);
                    ViewBag.isValid = false;
                }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string keyId, string VerificationCode, string Email)
        {
            string EncodedResponse = Request.Form["g-Recaptcha-Response"];
            if (EncodedResponse == "")
            {
                ViewBag.recapcha = true;
                return View();
            }
            if (ModelState.IsValid && keyId != null && VerificationCode != null && Email != null)
            {
                string URI = "";
                try
                {
                    using (var db = new eve())
                    {
                       URI = "https://www.google.com/recaptcha/api/siteverify?secret=" + Startup.recapcha + "&response=" + EncodedResponse;


                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: '{0}'", e);
                    telemetry.TrackException(e);
                    ViewBag.recapcha2 = true;
                    return View();
                }
                var test = new HttpClient();

                HttpResponseMessage response = await test.PostAsync(URI, new StringContent(""));
                HttpContent contentR = response.Content;

                // ... Read the string.
                string result = await contentR.ReadAsStringAsync();


                Recaptcha recapchaResult;

                    recapchaResult = JsonConvert.DeserializeObject<Recaptcha>(result);
                if (!recapchaResult.success)
                {
                    ViewBag.recapcha2 = true;
                    return View();
                }

                try
                {
                    ViewBag.mask = false;
                    ViewBag.character = false;
                    ViewBag.invalid = false;
                    ViewBag.exist = false;
                    ViewBag.email = false;
                    ViewBag.success = false;
                    var xml = XDocument.Load("https://api.eveonline.com/account/APIKeyInfo.xml.aspx?keyID=" + keyId + "&vCode=" + VerificationCode);

                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(apiCheck.eveapi));

                    // Declare an object variable of the type to be deserialized.
                    apiCheck.eveapi i;
                    using (var reader = xml.Root.CreateReader())
                    {
                        i = (apiCheck.eveapi)serializer.Deserialize(reader);
                    }


                    if (i.result.key.accessMask != 2560)
                    {
                        ViewBag.mask = true;
                    }
                    if (i.result.key.type != "Character")
                    {
                        ViewBag.character = true;
                    }
                    ViewBag.email = !IsValidEmail(Email);
                    if (ViewBag.mask || ViewBag.character || ViewBag.email)
                        return View();

                    using (var db = new eve())
                    {
                        var chatId = i.result.key.rowset.row.First().characterID.ToString();
                        if (db.Users.Where(x => x.character == chatId).Any())
                        {
                            ViewBag.exist = true;
                            return View();
                        }
                        var user = new Users
                        {
                            ApiId = keyId,
                            character = chatId,
                            ApiKey = VerificationCode,
                            Email = Email,
                            fails = 0,
                            HadFirst = false,
                            verified = false,
                            verifyUrl = Guid.NewGuid()
                        };
                        db.Users.Add(user);
                        await db.SaveChangesAsync();

                        await sendEmail(Email, user.verifyUrl.ToString(), Startup.sendGridUser, Startup.sendGridPass);
                    }

                    ViewBag.success = true;
                    return Redirect("Index?success=true");
                }
                catch (Exception e)
                {
                    try
                    {
                        throw new ArgumentException("register failed: email = " + Email + ", keyId = " + keyId + ", VerificationCode = " + VerificationCode + " exception: " + e.Message, e);
                    }
                    catch (Exception e2)
                    {
                        Console.WriteLine("An error occurred: '{0}'", e);
                        telemetry.TrackException(e2);
                        ViewBag.invalid = true;
                        return View();

                    }
                }
            }
            return View();
        }
        public async Task sendEmail(string email, string verifyString, string sendGridUser, string sendGridPass)
        {
            MailMessage mailMsg = new MailMessage();
            mailMsg.Priority = MailPriority.High;
            // To
            mailMsg.To.Add(new MailAddress(email, email));

            // From
            mailMsg.From = new MailAddress("info@evenotify.org", "eve notify");

            mailMsg.Subject = "Verification email";

            // Init SmtpClient and send
            SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(sendGridUser, sendGridPass);
            smtpClient.Credentials = credentials;

            mailMsg.Body += "<a target='_blank' href='http://evenotify.org/Home/mail?id=" + verifyString + "'>verify email</a> or go to this url: http://evenotify.org/Home/mail?id=" + verifyString + "<br/><br/> Please ignore this email if you did not request it.";

            mailMsg.IsBodyHtml = true;
            await smtpClient.SendMailAsync(mailMsg);
        }
       
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

       
    }
}
