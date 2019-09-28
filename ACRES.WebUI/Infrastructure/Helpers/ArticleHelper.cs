using ACRES.Domain.Context;
using ACRES.Domain.Entities;
using ACRES.WebUI.Models.News;
using MagicApps.Infrastructure.Services;
using MagicApps.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ACRES.WebUI.Infrastructure.Helpers
{
    public class ArticleHelper
    {
        private ApplicationDbContext db;
        private ApplicationUserManager UserManager;

        int ArticleId;

        public Article Article { get; private set; }

        public string ServiceUserId { get; set; }

        public ArticleHelper()
        {
            Set();
        }

        public ArticleHelper(int ArticleId)
        {
            Set();

            this.ArticleId = ArticleId;
            this.Article = db.Articles.Find(ArticleId);
        }

        public ArticleHelper(Article Article)
        {
            Set();

            this.ArticleId = Article.ArticleId;
            this.Article = Article;
        }

        private void Set()
        {
            this.db = HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();
            this.UserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }


        public async Task<UpsertModel> UpsertArticle(UpsertMode mode, NewsViewModel model)
        {
            var upsert = new UpsertModel();

            try
            {
                Activity activity;
                string title;
                System.Text.StringBuilder builder;

                // Apply changes
                Article = model.ParseAsEntity(Article);

                builder = new System.Text.StringBuilder();

                if (model.NewsId == 0)
                {
                    db.Articles.Add(Article);

                    title = "Article Created";
                    builder.Append("A Article record has been created titled: " + model.Title).AppendLine();
                }
                else
                {
                    db.Entry(Article).State = System.Data.Entity.EntityState.Modified;

                    title = "Article Updated";
                    builder.Append("Changes have been made to the Article details.");

                    if (mode == UpsertMode.Admin)
                    {
                        builder.Append(" (by the Admin)");
                    }

                    builder.Append(":").AppendLine();
                }

                await db.SaveChangesAsync();

                ArticleId = Article.ArticleId;

                UploadImage(model.Image);

                // Save activity now so we have a ArticleId. Not ideal, but hey
                activity = CreateActivity(title, builder.ToString());
                activity.UserId = ServiceUserId;

                await db.SaveChangesAsync();

                if (model.NewsId == 0)
                {
                    upsert.ErrorMsg = "Article record created successfully";
                }
                else
                {
                    upsert.ErrorMsg = "Article record updated successfully";
                }

                upsert.RecordId = Article.ArticleId.ToString();
            }
            catch (Exception ex)
            {
                upsert.ErrorMsg = ex.Message;
            }

            return upsert;
        }

        public async Task<UpsertModel> DeleteArticle()
        {
            var upsert = new UpsertModel();

            try
            {

                string title = "Article Deleted";
                System.Text.StringBuilder builder = new System.Text.StringBuilder()
                    .Append("The following Article has been deleted:")
                    .AppendLine()
                    .AppendLine().AppendFormat("Title: {0}", Article.Title)
                    .AppendLine().AppendFormat("Description: {0}", Article.Description)
                    .AppendLine().AppendFormat("Publish Date: {0}", Article.Published.ToString("ddd, dd MMM yyyy"));

                // Record activity
                var activity = CreateActivity(title, builder.ToString());
                activity.UserId = ServiceUserId;

                upsert.ErrorMsg = string.Format("Article titled '{0}' deleted successfully", Article.Title);
                upsert.RecordId = Article.ArticleId.ToString();

                // recall to delete the file aswell.
                DeleteImage();

                // Remove Article
                db.Articles.Remove(Article);


                await db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                upsert.ErrorMsg = ex.Message;
            }

            return upsert;
        }

        public Activity CreateActivity(string title, string description)
        {
            var activity = new Activity
            {
                Title = title,
                Description = description,
                RecordedById = ServiceUserId,
                ArticleId = ArticleId
            };
            db.Activities.Add(activity);
            return activity;
        }

        private bool UploadImage(HttpPostedFileBase image)
        {
            try
            {
                if (image != null)
                {
                    string folder = string.Format(@"~/Content/Imgs/Article-{0}", ArticleId);

                    FileService.CreateFolder(folder);

                    folder = string.Format(@"{0}\Article-{1}", ConfigurationManager.AppSettings["Settings.Site.ImgFolder"], ArticleId);

                    var fileExt = Path.GetExtension((image as HttpPostedFileBase).FileName).Substring(1);
                    string path = Path.Combine(folder, string.Format("Image.{0}", fileExt));

                    // delete the file if it exists
                    //FileService.DeleteFile(path);
                    image.SaveAs(path);
                }
            }
            catch (Exception)
            {               
                return false;
            }


            return true;
        }

        private bool DeleteImage()
        {
            try
            {
                var folder = string.Format(@"~/Content/Imgs/Article-{0}", ArticleId);
                FileService.DeleteFolder(folder);
            }
            catch (Exception)
            {
                return false;
            }


            return true;
        }


    }
}