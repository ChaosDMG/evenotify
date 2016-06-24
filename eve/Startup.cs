using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Timers;
using System.Xml.Linq;
using eve.Models;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.ApplicationInsights;
using System.Threading.Tasks;
using System.Diagnostics;

[assembly: OwinStartup(typeof(eve.Startup))]

namespace eve
{
    public partial class Startup
    {
        private static Timer timer;
        private static int throttleCounter = 0;
        private static TelemetryClient telemetry = new TelemetryClient();
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);


            timer = new Timer(600000);

            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Enabled = true;
            mailList();
        }
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            telemetry.TrackMetric("throttleCounter", throttleCounter);
            throttleCounter = 0;
            mailList();
            
        }
        private static void mailList()
        {
            try
            {
                using (var db = new eveEntity())
                {
                    foreach (var a in db.Users.ToList())
                    {
                        try
                        {
                            throttleCounter++;
                            //if (throttleCounter >= 30)
                            //{
                            //    await Task.Delay(1000);
                            //}
                            if (!a.verified)
                                throw new Exception("not verified");
                            var xml = XDocument.Load("https://api.eveonline.com/char/mailmessages.xml.aspx?keyID=" + a.ApiId + "&vCode=" + a.ApiKey);
                            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MailList.eveapi));

                            // Declare an object variable of the type to be deserialized.
                            MailList.eveapi i;
                            using (var reader = xml.Root.CreateReader())
                            {
                                i = (MailList.eveapi)serializer.Deserialize(reader);
                            }
                            string msgIDs = "";
                            int counter = 0;
                            foreach (var r in i.result.rowset.row)
                            {
                                string id = r.messageID.ToString();
                                if (db.MsgIds.Where(x => x.Id == id).Any())
                                {
                                }
                                else
                                {
                                    db.MsgIds.Add(new MsgId { Id = id });
                                    if (counter == 0)
                                        msgIDs += r.messageID;
                                    else
                                        msgIDs += "," + r.messageID;

                                    counter++;
                                }
                            }
                            if (a.HadFirst == false)
                            {

                                a.HadFirst = true;
                            }
                            else
                            {
                                if (msgIDs != "")
                                    msgs(i, msgIDs, a.ApiId, a.ApiKey, a.Email, db.PrivateKeys.First(x=>x.Name == "sendGridUser").KeyVar, db.PrivateKeys.First(x => x.Name == "sendGridPass").KeyVar);
                            }

                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("An error occurred: '{0}'", e);
                            telemetry.TrackException(e);

                            try
                            {
                                a.fails+=7;
                                if (e.Message.CompareTo("not verified") == 0)
                                    a.fails -= 6;
                                    if (a.fails > 22)
                                {
                                    db.Entry(a).State = System.Data.Entity.EntityState.Deleted;

                                    db.SaveChanges();
                                }
                                db.SaveChanges();
                            }
                            catch (Exception e2)
                            {
                                Console.WriteLine("An error occurred: '{0}'", e2);
                                telemetry.TrackException(e2);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
                telemetry.TrackException(e);
            }

        }

        private static async void msgs(MailList.eveapi i, string msgIDs, string apiId, string apiKey, string email, string sendGridUser, string sendGridPass)
        {
            try
            {
                throttleCounter++;
                //if (throttleCounter >= 30)
                //{
                //    await Task.Delay(1000);
                //}
                var xml2 = XDocument.Load("https://api.eveonline.com/char/mailbodies.xml.aspx?keyID=" + apiId + "&vCode=" + apiKey + "&IDs=" + msgIDs);

                System.Xml.Serialization.XmlSerializer serializer2 = new System.Xml.Serialization.XmlSerializer(typeof(MailMsg.eveapi));

                // Declare an object variable of the type to be deserialized.
                MailMsg.eveapi i2;
                using (var reader = xml2.Root.CreateReader())
                {
                    i2 = (MailMsg.eveapi)serializer2.Deserialize(reader);
                }
                MailMessage mailMsg = new MailMessage();
                mailMsg.Priority = MailPriority.High;
                // To
                mailMsg.To.Add(new MailAddress(email, email));

                // From
                mailMsg.From = new MailAddress("eve@jannesvh.com", "eve notify");

                mailMsg.Subject = "evemail";
                // Subject and multipart/alternative Body
                //string text = "kyk from address hehe :)";
                //string html = @"<p>kyk from address hehe :)</p>";
                //mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                //mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                // Init SmtpClient and send
                SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(sendGridUser, sendGridPass);
                smtpClient.Credentials = credentials;

                foreach (var r in i2.result.rowset.row)
                {
                    mailMsg.Body += "<div style='background-color: grey; padding: 10px'>From: " + i.result.rowset.row.Where(x => x.messageID == r.messageID).First().senderName
                         + "<br/>" + "Title: " + i.result.rowset.row.Where(x => x.messageID == r.messageID).First().title
                        + "<br/>" + r.Value + "</div><br/><br/>";
                }
                mailMsg.IsBodyHtml = true;
                await smtpClient.SendMailAsync(mailMsg);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
                telemetry.TrackException(e);
            }

        }

    }
}
