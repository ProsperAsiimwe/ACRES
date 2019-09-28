using ACRES.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACRES.WebUI.Models.GalleryUploads
{
    public class GalleryListViewModel
    {
        public IEnumerable<GalleryUpload> GalleyUploads{ get; set; }
    }
}