using ACRES.Domain.Entities;
using ACRES.WebUI.Models.ContactMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ACRES.WebUI.Controllers
{
    public class ContactMessageController : BaseController
    {
        // GET: ContactMessage
        public ActionResult Index()
        {
            ViewBag.Active = "Messages";

            var model = new ContactMessageListViewModel
            {
                ContactMessages = context.ContactMessages.ToList()
            };
            return View(model);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactMessage contactMessages = context.ContactMessages.Find(id);
            if (contactMessages == null)
            {
                return HttpNotFound();
            }
            return View(contactMessages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContactMessage contactMessages = context.ContactMessages.Find(id);
            context.ContactMessages.Remove(contactMessages);
            context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}