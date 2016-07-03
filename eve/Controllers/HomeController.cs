using eve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.ApplicationInsights;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace eve.Controllers
{
    public class HomeController : Controller
    {
        private TelemetryClient telemetry = new TelemetryClient();
        public ActionResult Index(bool success = false)
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

        public async Task<ActionResult> mail(string id)
        {
            ViewBag.isValid = true;
            if (id == null || id.Length != 48)
            {
                ViewBag.isValid = false;
            }
            else
                try
                {
                    using (var db = new eveEntity())
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
        public async System.Threading.Tasks.Task<ActionResult> Index(string keyId, string VerificationCode, string Email)
        {
            if (ModelState.IsValid && keyId != null && VerificationCode != null && Email != null)
            {
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

                    using (var db = new eveEntity())
                    {
                        var chatId = i.result.key.rowset.row.First().characterID.ToString();
                        if (db.Users.Where(x => x.character == chatId).Any())
                        {
                            ViewBag.exist = true;
                            return View();
                        }
                        var verifyString = GetUniqueKey(48);
                        db.Users.Add(new User
                        {
                            ApiId = keyId,
                            character = chatId,
                            ApiKey = VerificationCode,
                            Email = Email,
                            fails = 0,
                            HadFirst = false,
                            verified = false,
                            verifyUrl = verifyString
                        });
                        await db.SaveChangesAsync();

                        await sendEmail(Email, verifyString, db.PrivateKeys.First(x => x.Name == "sendGridUser").KeyVar, db.PrivateKeys.First(x => x.Name == "sendGridPass").KeyVar);
                    }

                    ViewBag.success = true;
                    return Redirect("Index?success=true");
                }
                catch (Exception e)
                {
                    try
                    {
                        throw new ArgumentException("register failed: email = " + Email + ", keyId = " + keyId + ", VerificationCode = " + VerificationCode +" exception: "+ e.Message, e);
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

        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
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
