using ACRES.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRES.Domain.Entities
{
  public  class GalleryUpload
    {
        [Key]
        public int GalleryUploadId { get; set; }

        public GalleryUpload()
        {
            UploadDate = UgandaDateTime.DateNow();
        }

        [Display(Name = "Title")]
        [StringLength(50)]
        public string Title { get; set; }

        [Display(Name = "Image")]
        [StringLength(1000)]
        public string FileName { get; set; }

       
        public DateTime UploadDate { get; set; }
    }
}
