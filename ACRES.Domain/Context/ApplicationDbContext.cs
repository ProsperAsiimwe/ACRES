using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using ACRES.Domain.Entities;
using ACRES.Domain.Models;

namespace ACRES.Domain.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Activity> Activities { get; set; }

        public DbSet<Download> Downloads { get; set; }

        public DbSet<BlogPost> BlogPosts { get; set; }
               
        public DbSet<GalleryUpload> GalleryUploads { get; set; }

        public DbSet<Publication> Publications { get; set; }

        public DbSet<Subscriber> Subscribers { get; set; }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Newsletter> Newsletters { get; set; }

        public DbSet<Inquiry> Inquiries { get; set; }

        public DbSet<ContactMessage> ContactMessages { get; set; }

        public DbSet<DownloadItem> PublicationDownloads { get; set; }

        public DbSet<Reference> References { get; set; }

        public DbSet<Log> Log { get; set; }

        

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public override int SaveChanges()
        {
            try {
                return base.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex) {
                var error = GetEntityValidationErrors(ex);
                throw new System.Data.Entity.Validation.DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    error, ex
                    ); // Add the original exception as the innerException
            }
        }

        public override async System.Threading.Tasks.Task<int> SaveChangesAsync()
        {
            try {
                return await base.SaveChangesAsync();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex) {
                var error = GetEntityValidationErrors(ex);
                throw new System.Data.Entity.Validation.DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    error, ex
                    ); // Add the original exception as the innerException
            }
        }

        private string GetEntityValidationErrors(System.Data.Entity.Validation.DbEntityValidationException ex)
        {
            var sb = new System.Text.StringBuilder();

            foreach (var failure in ex.EntityValidationErrors) {
                sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                foreach (var error in failure.ValidationErrors) {
                    sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

       
    }
}