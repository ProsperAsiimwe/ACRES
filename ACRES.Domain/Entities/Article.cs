using ACRES.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRES.Domain.Entities
{
    public class Article
    {
        public Article()
        {
            Published = UgandaDateTime.DateNow();
        }

        [Key]
        public int ArticleId { get; set; }

        [StringLength(1000)]
        public string Author { get; set; }

        public DateTime Published { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(10000)]
        public string Description { get; set; }

        [StringLength(1000)]
        public string UrlToImage { get; set; }

        public string GetImage()
        {
            return string.Format("/Content/Imgs/Article-{0}/{1}", ArticleId, Images()[0]);
        }

        public string Folder()
        {
            return string.Format(@"{0}\Article-{1}", ConfigurationManager.AppSettings["Settings.Site.ImgFolder"], ArticleId);
        }

        public string[] Images()
        {
            return Directory.Exists(Folder()) ? Directory.GetFiles(Folder()).Select(Path.GetFileName).ToArray() : new string[] { };
        }


    }
}