using ACRES.WebUI.Infrastructure;
using ACRES.WebUI.Infrastructure.Helpers;
using ACRES.WebUI.Models.Newsletters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ACRES.Domain.Entities;
using MagicApps.Infrastructure.Helpers;

namespace ACRES.WebUI.Controllers
{
    [RoutePrefix("Newsletters")]
    public class NewslettersController : BaseController
    {
        // GET: Newsletters
        public NewslettersController()
        {
            ViewBag.Area = "Newsletters";

        }

        // GET: Newsletters
        [Authorize(Roles = "Developer, Admin, Client")]
        [Route("Admin/Candidates/{id:int}/Newsletters", Order = 1)]
        [Route("{status:regex(^(completed|incomplete)$)}/Page-{page:int}", Order = 11)]
        [Route("{status:regex(^(completed|incomplete)$)}", Order = 12)]
        [Route("Page-{page:int}", Order = 13)]
        [Route("", Order = 21, Name = "Newsletters_Index")]
        public ActionResult Index(SearchNewslettersModel search, int page = 1)
        {

            System.Text.StringBuilder hBody = new System.Text.StringBuilder();

            hBody.AppendLine(@"<h1 class=""animated slideInDown"">P.T.C Newsletters</h1>")
                .AppendLine(@"<h2 class=""animated slideInUp"">Tax Related Newsletters</h2>");
            ViewBag.HeaderText = hBody.ToString();

            // Return all Newsletters
            // If not a post-back (i.e. initial load) set the searchModel to session
            if (Request.Form.Count <= 0)
            {
                if (search.IsEmpty() && Session["SearchNewslettersModel"] != null)
                {
                    search = (SearchNewslettersModel)Session["SearchNewslettersModel"];
                }
            }

            var helper = new NewsletterHelper();
            var model = helper.GetNewsletterList(search, search.ParsePage(page));

            Session["SearchNewslettersModel"] = search;

            return View(model);
        }


        // GET: Newsletters
        [Authorize(Roles = "Developer, Admin")]
        public ActionResult New()
        {

            if (!IsRoutingOK(null))
            {
                return RedirectOnError();
            }

            var model = GetNewsletterModel(null);

            return View(model);
        }

        [Authorize(Roles = "Developer, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(NewsletterViewModel model)
        {
            if (!IsRoutingOK(null))
            {
                return RedirectOnError();
            }

            bool success = await Upsert(null, model);

            if (success)
            {
                return RedirectOnError();
            }

            // If we got this far, an error occurred

            return View("New", model);
        }

        // GET: Newsletters
        [Authorize(Roles = "Developer, Admin, Technician, Client")]
        [Route("{NewsletterId:int}")]
        public ActionResult Show(int NewsletterId)
        {
            if (!IsRoutingOK(NewsletterId))
            {
                return RedirectOnError();
            }

            var Newsletter = GetNewsletter(NewsletterId);
            return View(Newsletter);
        }

        // GET: Newsletters
        [Authorize(Roles = "Developer, Admin")]
        [Route("{NewsletterId:int}/Edit")]
        public ActionResult Edit(int NewsletterId)
        {
            if (!IsRoutingOK(NewsletterId))
            {
                return RedirectOnError();
            }

            var model = GetNewsletterModel(NewsletterId);
            return View("New", model);
        }

        [Authorize(Roles = "Developer, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(int NewsletterId, NewsletterViewModel model)
        {
            if (!IsRoutingOK(NewsletterId))
            {
                return RedirectOnError();
            }

            bool success = await Upsert(NewsletterId, model);

            if (success)
            {
                return RedirectOnError();
            }

            // If we got this far, an error occurred
            return View("New", model);
        }

        // GET: Newsletters
        [Authorize(Roles = "Developer, Admin")]
        [Route("{NewsletterId:int}/Delete")]
        public ActionResult Delete(int NewsletterId)
        {
            if (!IsRoutingOK(NewsletterId))
            {
                return RedirectOnError();
            }

            System.Text.StringBuilder hBody = new System.Text.StringBuilder();

            hBody.AppendLine(@"<h1 class=""animated slideInDown"">Delete Newsletter</h1>")
                .AppendLine(string.Format(@"<h3 class=""animated slideInUp"">{0}</h3>", GetNewsletter(NewsletterId).Title));
            ViewBag.HeaderText = hBody.ToString();

            return View(GetNewsletter(NewsletterId));
        }

        [Authorize(Roles = "Developer, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Destroy(int NewsletterId)
        {
            if (!IsRoutingOK(NewsletterId))
            {
                return RedirectOnError();
            }

            var helper = GetHelper(NewsletterId);
            var upsert = await helper.DeleteNewsletter();

            if (upsert.i_RecordId() > 0)
            {
                ShowSuccess(upsert.ErrorMsg);
                return RedirectOnError();
            }

            // If we got this far, an error occurred
            ShowError(upsert.ErrorMsg);
            return RedirectToAction("Delete", new { NewsletterId = NewsletterId });
        }

        #region Child Actions


        public PartialViewResult GetBreadcrumb(int NewsletterId, bool mainAsLink = true)
        {
            var Newsletter = GetNewsletter(NewsletterId);
            ViewBag.MainAsLink = mainAsLink;
            return PartialView("Partials/_Breadcrumb", Newsletter);
        }

        public PartialViewResult GetSummary(int NewsletterId)
        {
            var Newsletter = GetNewsletter(NewsletterId);
            return PartialView("Partials/_Summary", Newsletter);
        }

        public PartialViewResult GetActivities(int NewsletterId)
        {
            var activities = context.Activities.ToList()
                .Where(x => x.NewsletterId == NewsletterId)
                .OrderBy(o => o.Recorded);
            return PartialView("Partials/_Activities", activities);
        }

        #endregion

        #region Controller Helpers

        private bool IsRoutingOK(int? NewsletterId)
        {

            // Check Newsletter
            if (NewsletterId.HasValue)
            {
                var Newsletter = context.Newsletters.SingleOrDefault(x => x.NewsletterId == NewsletterId);

                if (Newsletter == null)
                {
                    return false;
                }

            }

            return true;
        }

        private NewsletterViewModel GetNewsletterModel(int? NewsletterId)
        {
            NewsletterViewModel model;

            if (NewsletterId.HasValue)
            {
                var Newsletter = GetNewsletter(NewsletterId.Value);
                model = new NewsletterViewModel(Newsletter);
            }
            else
            {
                model = new NewsletterViewModel();
            }

            return model;
        }

        private async Task<bool> Upsert(int? NewsletterId, NewsletterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var helper = (NewsletterId.HasValue ? GetHelper(NewsletterId.Value) : new NewsletterHelper() { ServiceUserId = GetUserId() });
                var upsert = await helper.UpsertNewsletter(UpsertMode.Admin, model);
                //var upsert = helper.UpsertNewsletter(UpsertMode.Admin, model);

                if (upsert.i_RecordId() > 0)
                {
                    var Newsletter = helper.Newsletter;

                    //// send email to clients.
                    string message = GetNewsletterMsg(Newsletter);

                    // Record activity and send mail, set service to null so it says system
                    helper.ServiceUserId = null;

                    await helper.SendNewsletter(message, false);

                    ShowSuccess(upsert.ErrorMsg);

                    return true;
                }
                else
                {
                    ShowError(upsert.ErrorMsg);
                }
            }

            return false;
        }

        protected string GetNewsletterMsg(Newsletter Newsletter)
        {
            var controller = new ControllerHelper(this.ControllerContext);
            return controller.RenderRazorViewToString("Mail/_Newsletter", Newsletter);
        }

        private RedirectToRouteResult RedirectOnError()
        {
            return RedirectToAction("Index");
        }

        private RedirectToRouteResult RedirectOnSuccess(int NewsletterId)
        {
            return RedirectToAction("Show", new { NewsletterId = NewsletterId });
        }

        private NewsletterHelper GetHelper(int NewsletterId)
        {
            NewsletterHelper helper = new NewsletterHelper(NewsletterId);

            helper.ServiceUserId = GetUserId();

            return helper;
        }

        private NewsletterHelper GetHelper(Newsletter Newsletter)
        {
            var helper = new NewsletterHelper(Newsletter);

            helper.ServiceUserId = GetUserId();

            return helper;
        }

        private Newsletter GetNewsletter(int NewsletterId)
        {
            return context.Newsletters.Find(NewsletterId);
        }


        #endregion

    }
}