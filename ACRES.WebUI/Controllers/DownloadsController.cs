using ACRES.Domain.Entities;
using ACRES.WebUI.Infrastructure.Helpers;
using ACRES.WebUI.Models.Downloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACRES.WebUI.Controllers
{
    [RoutePrefix("Downloads")]
    public class DownloadsController : BaseController
    {

        public DownloadsController()
        {
            ViewBag.Area = "Downloads";

        }

        // GET: Downloads
        [Authorize(Roles = "Developer, Admin")]
        [Route("Admin/Candidates/{id:int}/Downloads", Order = 1)]
        [Route("{status:regex(^(completed|incomplete)$)}/Page-{page:int}", Order = 11)]
        [Route("{status:regex(^(completed|incomplete)$)}", Order = 12)]
        [Route("Page-{page:int}", Order = 13)]
        [Route("", Order = 21, Name = "Downloads_Index")]
        public ActionResult Index(SearchDownloadsModel search, int page = 1)
        {
           
            // Return all Downloads
            // If not a post-back (i.e. initial load) set the searchModel to session
            if (Request.Form.Count <= 0)
            {
                if (search.IsEmpty() && Session["SearchDownloadsModel"] != null)
                {
                    search = (SearchDownloadsModel)Session["SearchDownloadsModel"];
                }
            }

            var helper = new DownloadHelper();
            var model = helper.GetDownloadList(search, search.ParsePage(page));

            Session["SearchDownloadsModel"] = search;

            return View(model);
        }

        #region Controller Helpers

        private DownloadHelper GetHelper(int DownloadId)
        {
            DownloadHelper helper = new DownloadHelper(DownloadId);

            helper.ServiceUserId = GetUserId();

            return helper;
        }

        private DownloadHelper GetHelper(Download Download)
        {
            var helper = new DownloadHelper(Download);

            helper.ServiceUserId = GetUserId();

            return helper;
        }

        private DownloadHelper GetHelper()
        {
            var helper = new DownloadHelper();

            helper.ServiceUserId = GetUserId();

            return helper;
        }

        private Download GetDownload(int DownloadId)
        {
            return context.Downloads.Find(DownloadId);
        }

        #endregion
    }
}