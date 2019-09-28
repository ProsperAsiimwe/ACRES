using ACRES.WebUI.Models.Subscribers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ACRES.WebUI.Controllers
{
    [RoutePrefix("Subscribers")]
    public class SubscribersController : BaseController
    {
        // GET: Subscribers
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult Subscribe()
        {
            return PartialView(new SubscriberViewModel());
        }

        [Authorize(Roles = "Developer, Admin, Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SubscriberViewModel model, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                try
                {

                    var Subscriber = context.Subscribers.FirstOrDefault(x => x.Email == model.Email);

                    // Apply changes
                    Subscriber = model.ParseAsEntity(Subscriber);

                    if (model.SubscriberId == 0)
                    {
                        context.Subscribers.Add(Subscriber);

                    }

                    await context.SaveChangesAsync();
                    ShowSuccess("You have successfully subscribed to our newsletter.");

                }
                catch (Exception ex)
                {
                    ShowError("Subscription failed " + ex.Message);
                }
            }

            // If we got this far, an error occurred

            return RedirectToAction("Index", "Home");
        }
    }
}