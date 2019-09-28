using MagicApps.Infrastructure.Services;
using ACRES.Domain.Context;
using ACRES.Domain.Entities;
using ACRES.WebUI.Models.BlogPosts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ACRES.WebUI.Infrastructure.Helpers;
using ACRES.WebUI.Infrastructure;

namespace ACRES.WebUI.Controllers
{
    public class BlogPostController : BaseController
    {
       

        // IMPLEMENTING C.R.U.D FUNCTIONALITY START

        // GET: BlogPosts/Read
        public ActionResult Read()
        {
            ViewBag.Active = "Blog";

            var model = new BlogPostListViewModel
            {
                BlogPosts = context.BlogPosts.ToList()
            };
            return View(model);
        }

        //GET: BlogPost
        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //The ValidateAntiForgeryToken attribute helps prevent cross-site request forgery attacks. It requires a corresponding Html.AntiForgeryToken()
        public ActionResult Create(BlogPostViewModel b)
        {
            if (ModelState.IsValid)
            {
               
                    var blog = b.ParseAsEntity(new BlogPost());
                context.BlogPosts.Add(blog);
                context.SaveChanges();

                    string folder = @"~/Content/PhotoBlogPost";
                    FileService.CreateFolder(folder);

                    string fileName = Path.GetFileNameWithoutExtension(b.BlogImage.FileName);
                    string extension = Path.GetExtension(b.BlogImage.FileName).Substring(1);
                    folder = ConfigurationManager.AppSettings["Settings.Site.BlogFolder"];
                    fileName = string.Format("{0}.{1}", blog.BlogPostId, extension);
                    string path = Path.Combine(folder, fileName);

                    FileService.DeleteFile(path);
                    b.BlogImage.SaveAs(path);

                    string imageFolder = "~/Content/PhotoBlogPost";
                    string imageFolderPath = Path.Combine(imageFolder, fileName);

                    blog.FileName = fileName;
                context.Entry(blog).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();


                    return RedirectToAction("Read");
                
            }

            return View();
        }

        // Get: BlogPosts/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost posts = context.BlogPosts.Find(id);
            if (posts == null)
            {
                return HttpNotFound();
            }
            return View(posts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost posts = context.BlogPosts.Find(id);
            context.BlogPosts.Remove(posts);
            context.SaveChanges();

            return RedirectToAction("Read");
        }

        public ActionResult Edit(int? id)
        {
            if (!IsRoutingOK(id))
            {
                return RedirectOnError();
            }

            var model = GetBlogModel(id);

            //return View("New", model);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(int BlogPostId, BlogPostViewModel model)
        {
            if (!IsRoutingOK(BlogPostId))
            {
                return RedirectOnError();
            }
            bool success = await Upsert(BlogPostId, model);
            if (success)
            {
                return RedirectOnSuccess(BlogPostId);
            }

            //return View("New", model);

            return View(model);

        }


        // IMPLEMENTING C.R.U.D FUNCTIONALITY END

        private async Task<bool> Upsert(int? BlogPostId, BlogPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var helper = (BlogPostId.HasValue ? GetHelper(BlogPostId.Value) : new BlogHelper() { ServiceUserId = GetUserId() });
                var upsert = await helper.UpsertBlogPost(UpsertMode.Admin, model);

                if (upsert.i_RecordId() > 0)
                {
                    ShowSuccess(upsert.ErrorMsg);

                    return true;
                }
                else
                {
                    ShowError(upsert.ErrorMsg);
                }
            }

            //model.SetLists();
            return false;
        }

        private BlogHelper GetHelper(int BlogPostId)
        {
            BlogHelper helper = new BlogHelper(BlogPostId);

            helper.ServiceUserId = GetUserId();

            return helper;
        }

        private BlogPostViewModel GetBlogModel(int? BlogPostId)
        {
            BlogPostViewModel model;
            if (BlogPostId.HasValue)
            {
                var Blog = GetBlogPost(BlogPostId.Value);
                model = new BlogPostViewModel(Blog);
            }
            else
            {
                model = new BlogPostViewModel();
            }

            return model;
        }

        private BlogPost GetBlogPost(int BlogPostId)
        {
            return context.BlogPosts.Find(BlogPostId);
        }

        private bool IsRoutingOK(int? BlogPostId)
        {

            // Check Article
            if (BlogPostId.HasValue)
            {
                var blogPost = context.BlogPosts.SingleOrDefault(x => x.BlogPostId == BlogPostId);

                if (blogPost == null)
                {
                    return false;
                }

            }

            return true;
        }

        private RedirectToRouteResult RedirectOnError()
        {
            return RedirectToAction("Read");
        }

        private RedirectToRouteResult RedirectOnSuccess(int BlogPostId)
        {
            return RedirectToAction("Read", new { BlogPostId = BlogPostId });
        }

    }
}