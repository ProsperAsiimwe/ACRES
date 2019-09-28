using ACRES.Domain.Models;
using ACRES.WebUI.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACRES.WebUI.Controllers
{
    public class HomeAdminController : BaseController
    {
        // GET: HomeAdmin
        public ActionResult Index()
        {
            ViewBag.Active = "Home";

            Session["SearchUsersModel"] = null;
            Session["SearchPaymentsModel"] = null;
            Session["SearchSponseesModel"] = null;
            Session["SearchSponsorsModel"] = null;
            Session["SearchTermsModel"] = null;
            Session["SearchClassesModel"] = null;
            Session["SearchMessagesModel"] = null;


            var model = new DashboardModel();

            // fill up dashboard lists
            model.Activities = context.Activities
             .OrderByDescending(o => o.Recorded)
             .Take(20);

            //var term = context.Terms.FirstOrDefault(p => p.IsCurrentTerm);
            //model.TermId = term == null ? (int?)null : term.EndDate <= UgandaDateTime.DateNow().Date ? term.TermId : (int?)null;
            return View(model);

        }

        public ActionResult PublicPage()
        {

            return RedirectToAction("Index", "Home");
        }
    }
}