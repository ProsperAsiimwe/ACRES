using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACRES.WebUI.Models.Publications
{
    public class SearchPublicationsModel : ListModel
    {
        [Display(Name = "Title")]
        public string Name { get; set; }

        [Display(Name = "Publication name")]
        public string PublicationName { get; set; }

        [Display(Name = "Subject Matter")]
        public string SubjectMatter { get; set; }

        public int? Type { get; set; }

        [Display(Name = "Publication Id")]
        public int? PublicationId { get; set; }

        public string Status { get; set; }

        public int? ClientId { get; set; }

        public bool IsEmpty()
        {
            if (!String.IsNullOrEmpty(this.Name) || !String.IsNullOrEmpty(this.SubjectMatter) || PublicationId.HasValue || Type.HasValue)
            {
                return false;
            }
            else
            {
                return true;
            }
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