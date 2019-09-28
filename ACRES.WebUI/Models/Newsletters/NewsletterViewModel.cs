using ACRES.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACRES.WebUI.Models.Newsletters
{
    public class NewsletterViewModel
    {
        public NewsletterViewModel()
        {

        }

        public NewsletterViewModel(Newsletter entity)
        {
            NewsletterId = entity.NewsletterId;
            Title = entity.Title;
            News = entity.News;
        }

        public int NewsletterId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(5000)]
        public string News { get; set; }

        public Newsletter ParseAsEntity(Newsletter entity)
        {
            if (entity == null)
            {
                entity = new Newsletter();
            }

            entity.Title = Title;
            entity.News = News;

            return entity;
        }
    }
}