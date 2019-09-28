using MagicApps.Infrastructure.Services;
using ACRES.Domain.Entities;
using ACRES.WebUI.Models.GalleryUploads;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ACRES.WebUI.Controllers
{
    public class GalleryUploadController : BaseController
    {
        // GET: GalleryUpload
        public ActionResult Index()
        {
            ViewBag.Active = "Gallery";

            var model = new GalleryListViewModel
            {
                GalleyUploads = context.GalleryUploads.ToList()
            };
            return View(model);
        }

        public ActionResult New()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New(GalleryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                int count = viewModel.GalleryImages.Count();
                foreach (var image in viewModel.GalleryImages)
                {
                   
                if(image!=null){
                        if (image.ContentLength > 0)
                        {
                            if (Path.GetExtension(image.FileName).ToLower() == ".jpg"
                                || Path.GetExtension(image.FileName).ToLower() == ".png"
                                || Path.GetExtension(image.FileName).ToLower() == ".gif"
                                || Path.GetExtension(image.FileName).ToLower() == ".jpeg")
                            {
                               
                                var item = viewModel.ParseAsEntity(new GalleryUpload());
                                context.GalleryUploads.Add(item);
                                context.SaveChanges();

                                string folder = @"~/Content/PhotoGallery";
                                FileService.CreateFolder(folder);

                                string fileName = Path.GetFileNameWithoutExtension(image.FileName);
                                string extension = Path.GetExtension(image.FileName).Substring(1);
                                folder = ConfigurationManager.AppSettings["Settings.Site.GalleryFolder"];
                                fileName = string.Format("{0}.{1}", item.GalleryUploadId, extension);
                                string path = Path.Combine(folder, fileName);

                                FileService.DeleteFile(path);
                                image.SaveAs(path);
                                string imageFolder = "~/Content/PhotoGallery";
                                string imageFolderPath = Path.Combine(imageFolder, fileName);

                                item.FileName = fileName;                               
                                context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();

                                ViewBag.UploadSuccess = true;

                            }
                        }
                    }

                }

                ViewBag.Count = count;
                return RedirectToAction("Index");

            }

            return View();
            
        }

      

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GalleryUpload galleryUploads = context.GalleryUploads.Find(id);
            if (galleryUploads == null)
            {
                return HttpNotFound();
            }
            return View(galleryUploads);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GalleryUpload galleryUploads = context.GalleryUploads.Find(id);
            context.GalleryUploads.Remove(galleryUploads);
            context.SaveChanges();

            return RedirectToAction("Index");
        }



    }
}