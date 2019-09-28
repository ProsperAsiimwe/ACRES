using ACRES.Domain.Context;
using ACRES.Domain.Entities;
using ACRES.WebUI.Models.BlogPosts;
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
    public class BlogHelper
    {
        private ApplicationDbContext db;
        private ApplicationUserManager UserManager;

        int BlogPostId;

        public BlogPost BlogPost { get; private set; }

        public string ServiceUserId { get; set; }

        public BlogHelper()
        {
            Set();
        }

        public BlogHelper(int BlogPostId)
        {
            Set();

            this.BlogPostId = BlogPostId;
            this.BlogPost = db.BlogPosts.Find(BlogPostId);
        }

        public BlogHelper(BlogPost BlogPost)
        {
            Set();

            this.BlogPostId = BlogPost.BlogPostId;
            this.BlogPost = BlogPost;
        }

        private void Set()
        {
            this.db = HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();
            this.UserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        public async Task<UpsertModel> UpsertBlogPost(UpsertMode mode, BlogPostViewModel model)
        {
            var upsert = new UpsertModel();

            //try
            //{
            //Activity activity;

            //string title;
            System.Text.StringBuilder builder;

            // Apply changes
            BlogPost = model.ParseAsEntity(BlogPost);

            builder = new System.Text.StringBuilder();

            if (model.BlogPostId == 0)
            {
                db.BlogPosts.Add(BlogPost);

                //title = "Blog Created";
                builder.Append("A Blog record has been created for:").AppendLine();
            }
            else
            {
                db.Entry(BlogPost).State = System.Data.Entity.EntityState.Modified;

                //title = "Blog Updated";
                builder.Append("Changes have been made to the Blog details.");

                if (mode == UpsertMode.Admin)
                {
                    builder.Append(" (by the Admin)");
                }

                builder.Append(":").AppendLine();
            }

            await db.SaveChangesAsync();

            BlogPostId = BlogPost.BlogPostId;

            if (model.BlogImage != null)
            {
                UploadImage(model.BlogImage);
            }

            //if (!(model.Evidence == null || model.Evidence.Length == 0 || model.Evidence[0] == null))
            //{
            //    UploadEvidence(model.Evidence);
            //}

            // Save activity now so we have a productId. Not ideal, but hey

            //activity = CreateActivity(title, builder.ToString());
            //activity.UserId = ServiceUserId;

            await db.SaveChangesAsync();

            if (model.BlogPostId == 0)
            {
                upsert.ErrorMsg = "Blog record created successfully";
            }
            else
            {
                upsert.ErrorMsg = "Blog record updated successfully";
            }

            upsert.RecordId = BlogPost.BlogPostId.ToString();
            //}
            //catch (Exception ex)
            //{
            //    upsert.ErrorMsg = ex.Message;
            //}

            return upsert;
        }


        public async Task<UpsertModel> DeleteProduct()
        {
            var upsert = new UpsertModel();

            //try
            //{
            //string title = "Blog Deleted";
            System.Text.StringBuilder builder = new System.Text.StringBuilder()
                .Append("The following Product has been deleted:")
                .AppendLine()
                 .AppendLine().AppendFormat("Title: {0}", BlogPost.Title)
                .AppendLine().AppendFormat("Author: {0}", BlogPost.Author)
                .AppendLine().AppendFormat("Description: {0}", BlogPost.Content)

                .AppendLine().AppendFormat("Added: {0}", BlogPost.Date.ToString("ddd, dd MMM yyyy"));

            // Record activity

            //var activity = CreateActivity(title, builder.ToString());
            //activity.UserId = ServiceUserId;

            // Remove Product
            // this.Product.Terminated = UgandaDateTime.DateNow();
            db.BlogPosts.Remove(BlogPost);
            db.Entry(BlogPost).State = System.Data.Entity.EntityState.Modified;
            DeletePic();

            await db.SaveChangesAsync();

            upsert.ErrorMsg = string.Format("Blog: '{0}' terminated successfully", BlogPost.Title);
            upsert.RecordId = BlogPost.BlogPostId.ToString();
            //}
            //catch (Exception ex)
            //{
            //    upsert.ErrorMsg = ex.Message;
            //}

            return upsert;
        }




        private bool UploadImage(HttpPostedFileBase file)
        {
            //try
            //{

            if (file != null)
            {
               
                string folder = @"~/Content/PhotoBlogPost";
                FileService.CreateFolder(folder);

                string fileName = Path.GetFileNameWithoutExtension((file as HttpPostedFileBase).FileName);
                string extension = Path.GetExtension((file as HttpPostedFileBase).FileName).Substring(1);
                folder = ConfigurationManager.AppSettings["Settings.Site.BlogFolder"];
                fileName = string.Format("{0}.{1}", BlogPostId, extension);
                string path = Path.Combine(folder, fileName);
                string dbPath = Path.Combine(folder, BlogPost.FileName);

                FileService.DeleteFile(path);
                FileService.DeleteFile(dbPath);

                file.SaveAs(path);
                string imageFolder = "~/Content/PhotoBlogPost";
                string imageFolderPath = Path.Combine(imageFolder, fileName);

                BlogPost.FileName = fileName;
                //Product.FolderPath = imageFolderPath;
                db.Entry(BlogPost).State = System.Data.Entity.EntityState.Modified;


            }
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}


            return true;
        }

        private bool DeletePic()
        {
            //try
            //{
            var Pic = string.Format(@"~/Content/PhotoBlogPost/{0}", BlogPostId);
            var folder = ConfigurationManager.AppSettings["Settings.Site.BlogFolder"];
            folder = string.Format(@"~/Content/PhotoBlogPost", folder);

            string path = Path.Combine(folder, string.Format("{0}", Pic));

            FileService.DeleteFile(path);
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}


            return true;
        }

    }
}