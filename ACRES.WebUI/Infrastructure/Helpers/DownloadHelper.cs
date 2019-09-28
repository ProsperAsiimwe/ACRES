using ACRES.Domain.Context;
using ACRES.Domain.Entities;
using ACRES.WebUI.Models.Downloads;
using MagicApps.Infrastructure.Services;
using MagicApps.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using TwitterBootstrap2;

namespace ACRES.WebUI.Infrastructure.Helpers
{
    public class DownloadHelper
    {

        private ApplicationDbContext db;
        private ApplicationUserManager UserManager;

        int DownloadId;

        public Download Download { get; private set; }

        public string ServiceUserId { get; set; }

        public DownloadHelper()
        {
            Set();
        }

        public DownloadHelper(int DownloadId)
        {
            Set();

            this.DownloadId = DownloadId;
            this.Download = db.Downloads.Find(DownloadId);
        }

        public DownloadHelper(Download Download)
        {
            Set();

            this.DownloadId = Download.DownloadId;
            this.Download = Download;
        }

        private void Set()
        {
            this.db = HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();
            this.UserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        public DownloadListViewModel GetDownloadList(SearchDownloadsModel searchModel, int page = 1)
        {
            int pageSize = 20;

            if (page < 1)
            {
                page = 1;
            }

            IEnumerable<Download> records = db.Downloads.ToList();

            // Remove any default information
            //searchModel.ParseRouteInfo();

            if (!String.IsNullOrEmpty(searchModel.Name))
            {
                string name = searchModel.Name.ToLower();
                records = records.Where(x => !string.IsNullOrEmpty(x.ClientId) && (x.Client.FirstName.ToLower().Contains(name) || x.Client.LastName.ToLower().Contains(name)));
            }
            if (!String.IsNullOrEmpty(searchModel.SubjectMatter))
            {
                string name = searchModel.SubjectMatter.ToLower();
                records = records.Where(x => x.Contains(name));
            }


            return new DownloadListViewModel
            {
                Downloads = records
                    .OrderByDescending(o => o.Date)
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

        public Activity CreateActivity(string title, string description)
        {
            var activity = new Activity
            {
                Title = title,
                Description = description,
                RecordedById = ServiceUserId,
                DownloadId = DownloadId
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

                    FileService.CreateFolder(@"~/App_Data/Purchases");

                    folder = string.Format(@"{0}\Purchases", folder);

                    var fileExt = Path.GetExtension((file as HttpPostedFileBase).FileName).Substring(1);
                    string path = Path.Combine(folder, string.Format("{0}.{1}", fileName, fileExt));

                    // delete the file if it exists
                    FileService.DeleteFile(path);
                    file.SaveAs(path);
                }
            }
            catch (Exception )
            {
                return false;
            }


            return true;
        }

        private bool DeletePublicationFile()
        {
            try
            {
                var fileName = string.Format("ACRES_Download_{0}", Download);

                var folder = ConfigurationManager.AppSettings["Settings.Site.DocFolder"];

                folder = string.Format(@"{0}\Downloads", folder);

                string path = Path.Combine(folder, string.Format("{0}.{1}", fileName, "pdf"));

                // delete the file if it exists
                FileService.DeleteFile(path);
            }
            catch (Exception )
            {
                return false;
            }


            return true;
        }

    }
}