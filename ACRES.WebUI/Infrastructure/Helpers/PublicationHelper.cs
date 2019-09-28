using ACRES.Domain.Context;
using ACRES.Domain.Entities;
using ACRES.Domain.Models;
using ACRES.WebUI.Models.Publications;
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
using TwitterBootstrap2;

namespace ACRES.WebUI.Infrastructure.Helpers
{
    public class PublicationHelper
    {
        private ApplicationDbContext db;
        private ApplicationUserManager UserManager;

        int PublicationId;

        public Publication Publication { get; private set; }

        public string ServiceUserId { get; set; }

        public PublicationHelper()
        {
            Set();
        }

        public PublicationHelper(int PublicationId)
        {
            Set();

            this.PublicationId = PublicationId;
            this.Publication = db.Publications.Find(PublicationId);
        }

        public PublicationHelper(Publication Publication)
        {
            Set();

            this.PublicationId = Publication.PublicationId;
            this.Publication = Publication;
        }

        private void Set()
        {
            this.db = HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();
            this.UserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        public PublicationListViewModel GetPublicationList(SearchPublicationsModel searchModel, int page = 1)
        {
            int pageSize = 20;

            if (page < 1)
            {
                page = 1;
            }

            IEnumerable<Publication> records = db.Publications.ToList();

            // Remove any default information
            //searchModel.ParseRouteInfo();

            if (!String.IsNullOrEmpty(searchModel.PublicationName))
            {
                string name = searchModel.PublicationName.ToLower();
                records = records.Where(x => x.Title.ToLower().Contains(name));
            }
            if (!String.IsNullOrEmpty(searchModel.SubjectMatter))
            {
                string name = searchModel.SubjectMatter.ToLower();
                records = records.Where(x => x.SubjectMatter.ToString().ToLower().Contains(name));
            }
            if (searchModel.Type.HasValue)
            {
                records = records.Where(x => x.Type == searchModel.Type);
            }

            records = records.Where(x => !x.Deleted.HasValue);

            return new PublicationListViewModel
            {
                Publications = records
                    .OrderByDescending(o => o.Submitted)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize),
                SearchModel = searchModel,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalItems = records.Count()
                }
            };
        }

        public async Task<UpsertModel> UpsertPublication(UpsertMode mode, PublicationViewModel model)
        {
            var upsert = new UpsertModel();

            //try
            //{
            Activity activity;
            string title;
            System.Text.StringBuilder builder;

            var splitAt = Publication.SplitPage;

            // Apply changes
            Publication = model.ParseAsEntity(Publication);

            builder = new System.Text.StringBuilder();

            if (model.PublicationId == 0)
            {
                db.Publications.Add(Publication);

                title = "Publication Created";
                builder.Append("A Publication record has been created titled: " + model.Title).AppendLine();
            }
            else
            {
                db.Entry(Publication).State = System.Data.Entity.EntityState.Modified;

                title = "Publication Updated";
                builder.Append("Changes have been made to the Publication details.");

                if (mode == UpsertMode.Admin)
                {
                    builder.Append(" (by the Admin)");
                }

                builder.Append(":").AppendLine();
            }

            await db.SaveChangesAsync();

            PublicationId = Publication.PublicationId;

            if (model.File != null)
            {
                if (!string.Equals(Publication.Doc, model.File.FileName) || model.PublicationId <= 0)
                {
                    // delete old file, upload new one
                    DeletePublicationFile();

                    // upload file 
                    var fileName = string.Format("PTC_Publication_{0}", PublicationId);
                    var result = UploadPublicationFile(model.File, fileName);
                    if (result)
                    {
                        Publication.Doc = model.File.FileName;
                        db.Entry(Publication).State = System.Data.Entity.EntityState.Modified;
                    }

                }
            }

            if (model.PublicationId > 0 && model.SplitPage != splitAt && !string.IsNullOrEmpty(Publication.Doc))
            {
                // create preview here
                SplitPublication(model.SplitPage);
                builder.Append("Splitting now.");
            }

            // Save activity now so we have a PublicationId. Not ideal, but hey
            activity = CreateActivity(title, builder.ToString());
            activity.UserId = ServiceUserId;

            await db.SaveChangesAsync();

            if (model.PublicationId == 0)
            {
                upsert.ErrorMsg = "Publication record created successfully";
            }
            else
            {
                upsert.ErrorMsg = "Publication record updated successfully";
            }

            upsert.RecordId = Publication.PublicationId.ToString();
            //}
            //catch (Exception ex)
            //{
            //    upsert.ErrorMsg = ex.Message;
            //}

            return upsert;
        }

        public async Task<UpsertModel> DeletePublication()
        {
            var upsert = new UpsertModel();

            try
            {

                string title = "Publication Deleted";
                System.Text.StringBuilder builder = new System.Text.StringBuilder()
                    .Append("The following Publication has been deleted:")
                    .AppendLine()
                    .AppendLine().AppendFormat("Title: {0}", Publication.Title)
                    .AppendLine().AppendFormat("Subject: {0}", Publication.SubjectMatter)
                    .AppendLine().AppendFormat("Submission Date: {0}", Publication.Submitted.ToString("ddd, dd MMM yyyy"));

                // Record activity
                var activity = CreateActivity(title, builder.ToString());
                activity.UserId = ServiceUserId;

                upsert.ErrorMsg = string.Format("Publication titled '{0}' deleted successfully", Publication.Title);
                upsert.RecordId = Publication.PublicationId.ToString();

                // Remove Publication
                this.Publication.Deleted = UgandaDateTime.DateNow();
                db.Entry(Publication).State = System.Data.Entity.EntityState.Modified;

                // recall to delete the file aswell.
                DeletePublicationFile();

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
                PublicationId = PublicationId
            };
            db.Activities.Add(activity);
            return activity;
        }

        public static ButtonStyle GetButtonStyle(string css)
        {
            ButtonStyle button_css;

            if (css == "warning")
            {
                button_css = ButtonStyle.Warning;
            }
            else if (css == "success")
            {
                button_css = ButtonStyle.Success;
            }
            else if (css == "info")
            {
                button_css = ButtonStyle.Info;
            }
            else
            {
                button_css = ButtonStyle.Danger;
            }

            return button_css;
        }

        private bool UploadPublicationFile(HttpPostedFileBase file, string fileName)
        {
            try
            {
                if (file != null)
                {
                    var folder = ConfigurationManager.AppSettings["Settings.Site.DocFolder"];

                    FileService.CreateFolder(@"~/App_Data/Publications");

                    folder = string.Format(@"{0}\Publications", folder);

                    var fileExt = Path.GetExtension((file as HttpPostedFileBase).FileName).Substring(1);
                    string path = Path.Combine(folder, string.Format("{0}.{1}", fileName, fileExt));

                    // delete the file if it exists
                    FileService.DeleteFile(path);

                    // delete file preview aswell 
                    FileService.DeleteFile(string.Format("{0}-1", path));

                    // save file
                    file.SaveAs(path);

                    var outputFolder = ConfigurationManager.AppSettings["Settings.Site.UploadsFolder"];
                    // create preview
                    var pdfHelper = new PdfHelper(ServiceUserId);
                    var splitAt = Publication.SplitPage > 0 ? Publication.SplitPage : 1;
                    pdfHelper.Split(path, outputFolder, splitAt);
                }
            }
            catch (Exception)
            {
                return false;
            }


            return true;
        }

        private bool DeletePublicationFile()
        {
            try
            {
                var fileName = string.Format("PTC_Publication_{0}", PublicationId);

                var folder = ConfigurationManager.AppSettings["Settings.Site.DocFolder"];

                folder = string.Format(@"{0}\Publications", folder);

                string path = Path.Combine(folder, string.Format("{0}.{1}", fileName, "pdf"));

                // delete the file if it exists
                FileService.DeleteFile(path);

                // delete file preview aswell 
                FileService.DeleteFile(string.Format("{0}-1", path));
            }
            catch (Exception )
            {
                return false;
            }


            return true;
        }

        public bool SplitPublication(int splitAt)
        {
            try
            {
                var fileName = string.Format("PTC_Publication_{0}", PublicationId);
                var folder = string.Format(@"{0}\Publications", ConfigurationManager.AppSettings["Settings.Site.DocFolder"]);

                var fileExt = Path.GetExtension(Publication.Doc).Substring(1);
                string path = Path.Combine(folder, string.Format("{0}.{1}", fileName, fileExt));

                //FileService.DeleteFile(string.Format("{0}-1", path));

                // create preview
                var outputFolder = ConfigurationManager.AppSettings["Settings.Site.UploadsFolder"];

                var pdfHelper = new PdfHelper(ServiceUserId);
                pdfHelper.Split(path, outputFolder, splitAt);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}