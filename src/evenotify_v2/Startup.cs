using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Timers;
using Microsoft.ApplicationInsights;
using System.Xml.Linq;
using evenotify_v2.models;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;

namespace evenotify_v2
{
    public class Startup
    {
        public static string recapcha;
        public static string sendGridPass;
        public static string sendGridUser;

        private static Timer timer;
        private static int throttleCounter = 0;
        private static TelemetryClient telemetry = new TelemetryClient();
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();

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
        private static async void mailList()
        {
            try
            {
                using (var db = new eve())
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
                                    db.MsgIds.Add(new MsgIds { Id = id });
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
                                    msgs(i, msgIDs, a.ApiId, a.ApiKey, a.Email, Startup.sendGridUser, Startup.sendGridPass);
                            }
                            if (a.fails > 0)
                                a.fails--;
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("An error occurred: '{0}'", e);
                            telemetry.TrackException(e);

                            try
                            {
                                a.fails += 7;
                                if (e.Message.CompareTo("not verified") == 0)
                                    a.fails -= 6;
                                if (a.fails > 22)
                                {
                                    if (e.Message.CompareTo("not verified") != 0)
                                    {
                                        //todo create 1 moethod for all mails

                                        MailMessage mailMsg = new MailMessage();
                                        mailMsg.Priority = MailPriority.High;
                                        // To
                                        mailMsg.To.Add(new MailAddress(a.Email, a.Email));

                                        // From
                                        mailMsg.From = new MailAddress("info@evenotify.org", "eve notify");

                                        mailMsg.Subject = "evemail";
                                        SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                                        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(Startup.sendGridUser, Startup.sendGridPass);
                                        smtpClient.Credentials = credentials;

                                        mailMsg.Body += "Your API key was deleted for being invalid.  If you wish to still receive evemails as emails please register again.";
                                        mailMsg.IsBodyHtml = true;
                                        await smtpClient.SendMailAsync(mailMsg);

                                    }
                                    db.Entry(a).State = EntityState.Deleted;
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
                mailMsg.From = new MailAddress("info@evenotify.org", "eve notify");

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

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            recapcha = Configuration["Data:recapcha"];
            sendGridPass = Configuration["Data:sendGridPass"];
            sendGridUser = Configuration["Data:sendGridUser"];
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
           
        }
    }
}
