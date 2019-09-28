using ACRES.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACRES.WebUI.Models.Publications
{
    public class PublicationViewModel
    {
        public PublicationViewModel()
        {

        }

        public PublicationViewModel(Publication entity)
        {
            PublicationId = entity.PublicationId;
            Title = entity.Title;
            SubjectMatter = entity.SubjectMatter;
            Description = entity.Description;           
            Type = entity.Type;
            SplitPage = entity.SplitPage;
        }

        public int PublicationId { get; set; }

        public int Type { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [StringLength(100)]
        public string SubjectMatter { get; set; }

        [Required]
        public double Price { get; set; }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase File { get; set; }

        [UIHint("_Checkbox")]
        public bool IfOld { get; set; }

        [Display(Name = "Split Page Preview")]
        public int SplitPage { get; set; }

        public Publication ParseAsEntity(Publication entity)
        {
            if (entity == null)
            {
                entity = new Publication();
            }

            entity.Title = Title;
            entity.SubjectMatter = SubjectMatter;
            entity.Description = Description;            
            entity.Type = Type;
            entity.SplitPage = SplitPage;

            return entity;
        }

        public IDictionary<int, string> Types()
        {
            return new Dictionary<int, string>
            {
                {1, "Tax Publication" },
                {2, "Case Publication" }
            };
        }
    }
}