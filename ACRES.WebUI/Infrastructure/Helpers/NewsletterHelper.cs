using ACRES.Domain.Context;
using ACRES.Domain.Entities;
using ACRES.WebUI.Models.Newsletters;
using MagicApps.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TwitterBootstrap2;

namespace ACRES.WebUI.Infrastructure.Helpers
{
    public class NewsletterHelper
    {
        private ApplicationDbContext db;
        private ApplicationUserManager UserManager;

        int NewsletterId;

        public Newsletter Newsletter { get; private set; }

        public string ServiceUserId { get; set; }

        public NewsletterHelper()
        {
            Set();
        }

        public NewsletterHelper(int NewsletterId)
        {
            Set();

            this.NewsletterId = NewsletterId;
            this.Newsletter = db.Newsletters.Find(NewsletterId);
        }

        public NewsletterHelper(Newsletter Newsletter)
        {
            Set();

            this.NewsletterId = Newsletter.NewsletterId;
            this.Newsletter = Newsletter;
        }

        private void Set()
        {
            this.db = HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();
            this.UserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        public NewsletterListViewModel GetNewsletterList(SearchNewslettersModel searchModel, int page = 1)
        {
            int pageSize = 20;

            if (page < 1)
            {
                page = 1;
            }

            IEnumerable<Newsletter> records = db.Newsletters.ToList();

            // Remove any default information
            //searchModel.ParseRouteInfo();

            if (!String.IsNullOrEmpty(searchModel.NewsletterName))
            {
                string name = searchModel.NewsletterName.ToLower();
                records = records.Where(x => x.Title.ToLower().Contains(name));
            }

            return new NewsletterListViewModel
            {
                Newsletters = records
                    .OrderByDescending(o => o.Added)
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

        public async Task<UpsertModel> UpsertNewsletter(UpsertMode mode, NewsletterViewModel model)
        {
            var upsert = new UpsertModel();

            try
            {
                Activity activity;
                string title;
                System.Text.StringBuilder builder;

                // Apply changes
                Newsletter = model.ParseAsEntity(Newsletter);

                builder = new System.Text.StringBuilder();

                if (model.NewsletterId == 0)
                {
                    db.Newsletters.Add(Newsletter);

                    title = "Newsletter Created";
                    builder.Append("A Newsletter record has been created titled: " + model.Title).AppendLine();
                }
                else
                {
                    db.Entry(Newsletter).State = System.Data.Entity.EntityState.Modified;

                    title = "Newsletter Updated";
                    builder.Append("Changes have been made to the Newsletter details.");

                    if (mode == UpsertMode.Admin)
                    {
                        builder.Append(" (by the Admin)");
                    }

                    builder.Append(":").AppendLine();
                }

                await db.SaveChangesAsync();

                NewsletterId = Newsletter.NewsletterId;

                // Save activity now so we have a NewsletterId. Not ideal, but hey
                activity = CreateActivity(title, builder.ToString());
                activity.UserId = ServiceUserId;

                await db.SaveChangesAsync();

                if (model.NewsletterId == 0)
                {
                    upsert.ErrorMsg = "Newsletter record created successfully";
                }
                else
                {
                    upsert.ErrorMsg = "Newsletter record updated successfully";
                }

                upsert.RecordId = Newsletter.NewsletterId.ToString();
            }
            catch (Exception ex)
            {
                upsert.ErrorMsg = ex.Message;
            }

            return upsert;
        }

        public async Task<UpsertModel> DeleteNewsletter()
        {
            var upsert = new UpsertModel();

            try
            {

                string title = "Newsletter Deleted";
                System.Text.StringBuilder builder = new System.Text.StringBuilder()
                    .Append("The following Newsletter has been deleted:")
                    .AppendLine()
                    .AppendLine().AppendFormat("Title: {0}", Newsletter.Title)
                    .AppendLine().AppendFormat("Added: {0}", Newsletter.Added.ToString("ddd, dd MMM yyyy"));

                // Record activity
                var activity = CreateActivity(title, builder.ToString());
                activity.UserId = ServiceUserId;

                upsert.ErrorMsg = string.Format("Newsletter titled '{0}' deleted successfully", Newsletter.Title);
                upsert.RecordId = Newsletter.NewsletterId.ToString();

                // Remove Newsletter
                db.Newsletters.Remove(Newsletter);

                await db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                upsert.ErrorMsg = ex.Message;
            }

            return upsert;
        }

        public async Task<UpsertModel> SendNewsletter(string message, bool inTransaction)
        {
            var upsert = new UpsertModel();

            try
            {
                string title = "Newsletter sent";
                string description = string.Format("{0} Newsletter sent", Newsletter.Title);

                // Attach Newslettersheet and Send mail
                var mail = GetMailHelper();
                string subject = string.Format("{0} - Newsletter", Settings.COMPANY_NAME);
                var emails = db.Subscribers.Where(x => !x.Cancelled.HasValue).Select(p => p.Email).ToList();
                string status = string.Join(":", mail.SendMail(subject, message, emails));

                // record any errors
                mail.RecordErrors();

                // Record activity
                var activity = CreateActivity(title, string.Format("{0} - Status:{1}", description, status));
                activity.UserId = ServiceUserId;

                if (!inTransaction)
                {
                    await db.SaveChangesAsync();
                }

                upsert.ErrorMsg = upsert.ErrorMsg;
                upsert.RecordId = Newsletter.NewsletterId.ToString();
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
                NewsletterId = NewsletterId
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

        private MailHelper GetMailHelper()
        {
            var mail = new MailHelper(ServiceUserId);

            mail.NewsletterId = NewsletterId;
            mail.UserId = ServiceUserId;

            return mail;
        }
    }
}