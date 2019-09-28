using ACRES.Domain.Entities;
using ACRES.WebUI.Infrastructure;
using ACRES.WebUI.Infrastructure.Helpers;
using ACRES.WebUI.Models.Publications;
using MagicApps.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ACRES.WebUI.Controllers
{
    [RoutePrefix("Publications")]
    public class PublicationsController : BaseController
    {
        // GET: Publications
        public PublicationsController()
        {
            ViewBag.Area = "Publications";

        }

        // GET: Publications
        //[Authorize(Roles = "Developer, Admin, Client")]
        [Route("Page-{page:int}", Order = 13)]
        [Route("", Order = 21, Name = "Publications_Index")]
        public ActionResult Index(SearchPublicationsModel search, int page = 1)
        {

            System.Text.StringBuilder hBody = new System.Text.StringBuilder();

            hBody.AppendLine(@"<h1 class=""animated slideInDown"">P.T.C Publications</h1>")
                .AppendLine(@"<h2 class=""animated slideInUp"">Library of Tax Publications</h2>");
            ViewBag.HeaderText = hBody.ToString();

            // Return all Publications
            // If not a post-back (i.e. initial load) set the searchModel to session
            if (Request.Form.Count <= 0)
            {
                if (search.IsEmpty() && Session["SearchPublicationsModel"] != null)
                {
                    search = (SearchPublicationsModel)Session["SearchPublicationsModel"];
                }
            }

            var helper = new PublicationHelper();
            var model = helper.GetPublicationList(search, search.ParsePage(page));
            Session["SearchPublicationsModel"] = search;

            return View(model);
        }


        // GET: Publications
        [Authorize(Roles = "Developer, Admin")]
        public ActionResult New()
        {

            if (!IsRoutingOK(null))
            {
                return RedirectOnError();
            }

            var model = GetPublicationModel(null);

            return View(model);
        }

        [Authorize(Roles = "Developer, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PublicationViewModel model)
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

        // GET: Publications
        //[Authorize(Roles = "Developer, Admin, Technician, Client")]
        [Route("{PublicationId}/Show")]
        public ActionResult Show(int PublicationId)
        {
            if (!IsRoutingOK(PublicationId))
            {
                return RedirectOnError();
            }

            var Publication = GetPublication(PublicationId);

            //if (Publication.Doc != null)
            //{

            //    var folder = string.Format(@"{0}\Publications", ConfigurationManager.AppSettings["Settings.Site.DocFolder"]);
            //    // create preview
            //    //var pdfHelper = new PdfHelper(ServiceUserId);
            //    //var splitAt = Publication.SplitPage > 0 ? Publication.SplitPage : 1;
            //    //pdfHelper.Split(path, folder, splitAt);
            //}

            return PartialView("Partials/_Show", Publication);
        }

        // GET: Publications
        [Authorize(Roles = "Developer, Admin")]
        [Route("{PublicationId:int}/Edit")]
        public ActionResult Edit(int PublicationId)
        {
            if (!IsRoutingOK(PublicationId))
            {
                return RedirectOnError();
            }

            var model = GetPublicationModel(PublicationId);
            return View("New", model);
        }

        [Authorize(Roles = "Developer, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(int PublicationId, PublicationViewModel model)
        {
            if (!IsRoutingOK(PublicationId))
            {
                return RedirectOnError();
            }

            bool success = await Upsert(PublicationId, model);

            if (success)
            {
                return RedirectOnError();
            }

            // If we got this far, an error occurred
            return View("New", model);
        }

        // GET: Publications
        [Authorize(Roles = "Developer, Admin")]
        [Route("{PublicationId:int}/Delete")]
        public ActionResult Delete(int PublicationId)
        {
            if (!IsRoutingOK(PublicationId))
            {
                return RedirectOnError();
            }

            System.Text.StringBuilder hBody = new System.Text.StringBuilder();

            hBody.AppendLine(@"<h1 class=""animated slideInDown"">Delete Publication</h1>")
                .AppendLine(string.Format(@"<h3 class=""animated slideInUp"">{0}</h3>", GetPublication(PublicationId).Title));
            ViewBag.HeaderText = hBody.ToString();

            return View(GetPublication(PublicationId));
        }

        [Authorize(Roles = "Developer, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Destroy(int PublicationId)
        {
            if (!IsRoutingOK(PublicationId))
            {
                return RedirectOnError();
            }

            var helper = GetHelper(PublicationId);
            var upsert = await helper.DeletePublication();

            if (upsert.i_RecordId() > 0)
            {
                ShowSuccess(upsert.ErrorMsg);
                return RedirectOnError();
            }

            // If we got this far, an error occurred
            ShowError(upsert.ErrorMsg);
            return RedirectToAction("Delete", new { PublicationId = PublicationId });
        }

        #region Child Actions


        public PartialViewResult GetBreadcrumb(int PublicationId, bool mainAsLink = true)
        {
            var Publication = GetPublication(PublicationId);
            ViewBag.MainAsLink = mainAsLink;
            return PartialView("Partials/_Breadcrumb", Publication);
        }

        public PartialViewResult GetSummary(int PublicationId)
        {
            var Publication = GetPublication(PublicationId);
            return PartialView("Partials/_Summary", Publication);
        }

        public PartialViewResult GetActivities(int PublicationId)
        {
            var activities = context.Activities.ToList()
                .Where(x => x.PublicationId == PublicationId)
                .OrderBy(o => o.Recorded);
            return PartialView("Partials/_Activities", activities);
        }

        #endregion

        #region Controller Helpers

        private bool IsRoutingOK(int? PublicationId)
        {

            // Check Publication
            if (PublicationId.HasValue)
            {
                var Publication = context.Publications.SingleOrDefault(x => x.PublicationId == PublicationId);

                if (Publication == null)
                {
                    return false;
                }

            }

            return true;
        }

        private PublicationViewModel GetPublicationModel(int? PublicationId)
        {
            PublicationViewModel model;

            if (PublicationId.HasValue)
            {
                var Publication = GetPublication(PublicationId.Value);
                model = new PublicationViewModel(Publication);
            }
            else
            {
                model = new PublicationViewModel();
            }

            return model;
        }

        private async Task<bool> Upsert(int? PublicationId, PublicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var helper = (PublicationId.HasValue ? GetHelper(PublicationId.Value) : new PublicationHelper() { ServiceUserId = GetUserId() });
                var upsert = await helper.UpsertPublication(UpsertMode.Admin, model);
                //var upsert = helper.UpsertPublication(UpsertMode.Admin, model);

                if (upsert.i_RecordId() > 0)
                {
                    var Publication = helper.Publication;

                    //// send email to client.
                    //string message = GetPublicationMsg(Publication);

                    //// Record activity and send mail, set service to null so it says system
                    //helper.ServiceUserId = null;

                    //await helper.SendPublicationNotification(message, false);

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

        protected string GetPublicationMsg(Publication Publication)
        {
            var controller = new ControllerHelper(this.ControllerContext);
            return controller.RenderRazorViewToString("Mail/_NotifyPublication", Publication);
        }

        private RedirectToRouteResult RedirectOnError()
        {
            return RedirectToAction("Index");
        }

        private RedirectToRouteResult RedirectOnSuccess(int PublicationId)
        {
            return RedirectToAction("Show", new { PublicationId = PublicationId });
        }

        private PublicationHelper GetHelper(int PublicationId)
        {
            PublicationHelper helper = new PublicationHelper(PublicationId);

            helper.ServiceUserId = GetUserId();

            return helper;
        }

        private PublicationHelper GetHelper(Publication Publication)
        {
            var helper = new PublicationHelper(Publication);

            helper.ServiceUserId = GetUserId();

            return helper;
        }

        private Publication GetPublication(int PublicationId)
        {
            return context.Publications.Find(PublicationId);
        }


        #endregion

    }
}