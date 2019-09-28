using ACRES.Domain.Context;
using ACRES.Domain.Entities;
using ACRES.Domain.Models;
using ACRES.WebUI.Infrastructure.Helpers;
using ACRES.WebUI.Models.BlogPosts;
using ACRES.WebUI.Models.ContactMessages;
using ACRES.WebUI.Models.Dashboard;
using ACRES.WebUI.Models.GalleryUploads;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACRES.WebUI.Controllers
{
    public class HomeController : BaseController
    {
       

        public ActionResult Index()
        {
            ViewBag.Active = "Home";

            return View();
        }


        public ActionResult About()
        {
            ViewBag.Active = "About";
            return View();

        }

        public ActionResult Gallery()
        {
            ViewBag.Active = "Gallery";

            var model = new GalleryListViewModel
            {
                GalleyUploads = context.GalleryUploads.ToList()
            };
            return View(model);
        }

        public ActionResult BecomeASponsor()
        {
            ViewBag.Active = "Sponsor";

            return View();
        }

     
        public ActionResult Blog()
        {
            ViewBag.Active = "Blog";

            var model = new BlogPostListViewModel
            {
                BlogPosts = context.BlogPosts.ToList()
            };
            return View(model);
        }

        public ActionResult Contact()
        {
            ViewBag.Active = "Contact";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactMessageViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
               
                    var msg = viewModel.ParseAsEntity(new ContactMessage());
                    context.ContactMessages.Add(msg);
                    context.SaveChanges();

                    // send email
                    var mail = GetMailHelper();
                    string subject = string.Format("{0} - Contact Message", msg.Name);
                    string message = ContactMsgNotification(msg);
                    string status = string.Join(":", mail.SendMail(subject, message, ConfigurationManager.AppSettings["Settings.Company.Email"]));
                    mail.RecordErrors();

                    return RedirectToAction("Thanks");
               
            }

            return View();
        }


        public ActionResult Quotation()
        {

            return View();
        }

      
        public ActionResult Support()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Support(SupportRequestViewModel s)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        using (context)
        //        {
        //            var support = s.ParseAsEntity(new SupportRequest());
        //            context.SupportRequests.Add(support);
        //            context.SaveChanges();

        //            // send email
        //            var mail = GetMailHelper();
        //            string subject = string.Format("{0} - Support request", support.Name);
        //            //string message = SupportNotificationMsg(support);
        //            string status = string.Join(":", mail.SendMail(subject, message, ConfigurationManager.AppSettings["Settings.Company.Email"]));
        //            mail.RecordErrors();

        //            return RedirectToAction("Thanks");
        //        }

        //    }

        //    return View();
        //}

        public ActionResult AdminPage()
        {

            return RedirectToAction("Index", "Account");
        }

        public ActionResult Thanks()
        {
            return View();
        }

        public MailHelper GetMailHelper()
        {
            MailHelper mail = new MailHelper(null);
            mail.UserId = null;

            return mail;
        }

    }
}