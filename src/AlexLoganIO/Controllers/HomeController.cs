using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AlexLoganIO.Models;
using MimeKit;
using MailKit.Net.Smtp;

namespace AlexLoganIO.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Home";
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Title = "About";
            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Title = "Contact";
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = Startup.Configuration["AppSettings:SiteEmailAddress"];

                if (string.IsNullOrWhiteSpace(email))
                {
                    ModelState.AddModelError("", "Could not send email, configuration problem.");
                }

                ModelState.Clear();

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(model.Name, model.Email));
                message.To.Add(new MailboxAddress("Alex Logan", "alex@alexlogan.io"));
                message.Subject = "Message from site";
                message.Body = new TextPart("plain") { Text = model.Message };

                using(var client = new SmtpClient())
                {
                    client.Connect("smtp.office365.com", 587, false);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Send(message);
                    client.Disconnect(true);
                }

                ViewBag.Message = "Mail Sent. Thanks!";
            }

            return View();
        }
    }
}
