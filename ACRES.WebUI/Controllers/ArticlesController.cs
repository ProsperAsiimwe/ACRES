using ACRES.Domain.Entities;
using ACRES.Domain.Models;
using ACRES.WebUI.Infrastructure;
using ACRES.WebUI.Infrastructure.Helpers;
using ACRES.WebUI.Models.News;
using MagicApps.Infrastructure.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ACRES.WebUI.Controllers
{
    [RoutePrefix("Articles")]
    public class ArticlesController : BaseController
    {
        // GET: Articles
        public ArticlesController()
        {
            ViewBag.Area = "Articles";

        }

        // GET: Articles
        //[Authorize(Roles = "Developer, Admin, Client")]
        [Route("Page-{page:int}", Order = 13)]
        [Route("", Order = 21, Name = "Articles_Index")]
        public ActionResult Index()
        {

            System.Text.StringBuilder hBody = new System.Text.StringBuilder();

            hBody.AppendLine(@"<h1 class=""animated slideInDown"">Taxation News</h1>")
                .AppendLine(@"<h2 class=""animated slideInUp"">Stay tuned<br>And enjoy our posts.</h2>");

            ViewBag.HeaderText = hBody.ToString();

            var url = string.Format("https://newsapi.org/v2/everything?q=Taxation&from={0}&sortBy=popularity&apiKey=dedd35ef4a33423c8c8629813b84b00a", UgandaDateTime.DateNow().AddDays(-5).ToString("yyyy-MM-dd"));


            var json = new WebClient().DownloadString(url);

            var model = JsonConvert.DeserializeObject<ArticlesViewModel>(json);

            model.Articles.AddRange(context.Articles.ToList().Select(x => new NewsViewModel(x)));

            model.Articles = model.Articles.OrderByDescending(p => p.PublishedAt).ToList();

            return View(model);
        }


        // GET: Articles
        [Authorize(Roles = "Developer, Admin")]
        public ActionResult New()
        {

            if (!IsRoutingOK(null))
            {
                return RedirectOnError();
            }

            var model = GetArticleModel(null);

            return View(model);
        }

        [Authorize(Roles = "Developer, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(NewsViewModel model)
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

        // GET: Articles
        [Authorize(Roles = "Developer, Admin, Technician, Client")]
        [Route("{ArticleId}/Show")]
        public ActionResult Show(int ArticleId)
        {
            if (!IsRoutingOK(ArticleId))
            {
                return RedirectOnError();
            }

            var Article = GetArticle(ArticleId);
            return PartialView("Partials/_Show", Article);
        }

        // GET: Articles
        [Authorize(Roles = "Developer, Admin")]
        [Route("{ArticleId:int}/Edit")]
        public ActionResult Edit(int ArticleId)
        {
            if (!IsRoutingOK(ArticleId))
            {
                return RedirectOnError();
            }

            var model = GetArticleModel(ArticleId);
            return View("New", model);
        }

        [Authorize(Roles = "Developer, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(int ArticleId, NewsViewModel model)
        {
            if (!IsRoutingOK(ArticleId))
            {
                return RedirectOnError();
            }

            bool success = await Upsert(ArticleId, model);

            if (success)
            {
                return RedirectOnError();
            }

            // If we got this far, an error occurred
            return View("New", model);
        }

        // GET: Articles
        [Authorize(Roles = "Developer, Admin")]
        [Route("{ArticleId:int}/Delete")]
        public ActionResult Delete(int ArticleId)
        {
            if (!IsRoutingOK(ArticleId))
            {
                return RedirectOnError();
            }

            System.Text.StringBuilder hBody = new System.Text.StringBuilder();

            hBody.AppendLine(@"<h1 class=""animated slideInDown"">Delete Article</h1>")
                .AppendLine(string.Format(@"<h3 class=""animated slideInUp"">{0}</h3>", GetArticle(ArticleId).Title));
            ViewBag.HeaderText = hBody.ToString();

            return View(GetArticle(ArticleId));
        }

        [Authorize(Roles = "Developer, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Destroy(int ArticleId)
        {
            if (!IsRoutingOK(ArticleId))
            {
                return RedirectOnError();
            }

            var helper = GetHelper(ArticleId);
            var upsert = await helper.DeleteArticle();

            if (upsert.i_RecordId() > 0)
            {
                ShowSuccess(upsert.ErrorMsg);
                return RedirectOnError();
            }

            // If we got this far, an error occurred
            ShowError(upsert.ErrorMsg);
            return RedirectToAction("Delete", new { ArticleId = ArticleId });
        }

        #region Child Actions


        public PartialViewResult GetBreadcrumb(int ArticleId, bool mainAsLink = true)
        {
            var Article = GetArticle(ArticleId);
            ViewBag.MainAsLink = mainAsLink;
            return PartialView("Partials/_Breadcrumb", Article);
        }

        public PartialViewResult GetSummary(int ArticleId)
        {
            var Article = GetArticle(ArticleId);
            return PartialView("Partials/_Summary", Article);
        }

        public PartialViewResult GetActivities(int ArticleId)
        {
            var activities = context.Activities.ToList()
                .Where(x => x.ArticleId == ArticleId)
                .OrderBy(o => o.Recorded);
            return PartialView("Partials/_Activities", activities);
        }

        #endregion

        #region Controller Helpers

        private bool IsRoutingOK(int? ArticleId)
        {

            // Check Article
            if (ArticleId.HasValue)
            {
                var Article = context.Articles.SingleOrDefault(x => x.ArticleId == ArticleId);

                if (Article == null)
                {
                    return false;
                }

            }

            return true;
        }

        private NewsViewModel GetArticleModel(int? ArticleId)
        {
            NewsViewModel model;

            if (ArticleId.HasValue)
            {
                var Article = GetArticle(ArticleId.Value);
                model = new NewsViewModel(Article);
            }
            else
            {
                model = new NewsViewModel();
            }

            return model;
        }

        private async Task<bool> Upsert(int? ArticleId, NewsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var helper = (ArticleId.HasValue ? GetHelper(ArticleId.Value) : new ArticleHelper() { ServiceUserId = GetUserId() });
                var upsert = await helper.UpsertArticle(UpsertMode.Admin, model);
                //var upsert = helper.UpsertArticle(UpsertMode.Admin, model);

                if (upsert.i_RecordId() > 0)
                {
                    var Article = helper.Article;

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

        protected string GetArticleMsg(Article Article)
        {
            var controller = new ControllerHelper(this.ControllerContext);
            return controller.RenderRazorViewToString("Mail/_NotifyArticle", Article);
        }

        private RedirectToRouteResult RedirectOnError()
        {
            return RedirectToAction("Index");
        }

        private RedirectToRouteResult RedirectOnSuccess(int ArticleId)
        {
            return RedirectToAction("Show", new { ArticleId = ArticleId });
        }

        private ArticleHelper GetHelper(int ArticleId)
        {
            ArticleHelper helper = new ArticleHelper(ArticleId);

            helper.ServiceUserId = GetUserId();

            return helper;
        }

        private ArticleHelper GetHelper(Article Article)
        {
            var helper = new ArticleHelper(Article);

            helper.ServiceUserId = GetUserId();

            return helper;
        }

        private Article GetArticle(int ArticleId)
        {
            return context.Articles.Find(ArticleId);
        }


        #endregion

    }
}