using ACRES.Domain.Entities;
using ACRES.WebUI.Infrastructure;
using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACRES.WebUI.Models.News
{
    public class NewsViewModel
    {
        public NewsViewModel()
        {
            Source = new News.Source();
        }

        public NewsViewModel(Article entity)
        {
            NewsId = entity.ArticleId;
            Author = entity.Author;
            Title = entity.Title;
            Description = entity.Description;
            PublishedAt = entity.Published;
            DbSource = true;
            Source = new Source { name = Settings.COMPANY_ABBR };
            urlToImage = !string.IsNullOrEmpty(entity.UrlToImage) ? entity.UrlToImage : entity.GetImage();
        }

        public int NewsId { get; set; }

        public string Author { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [RequiredIf("Image", null)]
        [Display(Name = "Url to Image")]
        public string urlToImage { get; set; }

        public string Url { get; set; }

        public bool DbSource { get; set; }

        [RequiredIf("urlToImage", null)]
        public HttpPostedFileBase Image { get; set; }

        public DateTime PublishedAt { get; set; }

        public Source Source { get; set; }

        public Article ParseAsEntity(Article entity)
        {
            if (entity == null)
            {
                entity = new Article();
            }

            entity.Author = Author;
            entity.Title = Title;
            entity.Description = Description;
            entity.UrlToImage = urlToImage;

            return entity;
        }
    }

    public class Source
    {
        public string id { get; set; }

        public string name { get; set; }
    }
}
