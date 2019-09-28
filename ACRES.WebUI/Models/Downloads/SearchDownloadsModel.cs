using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACRES.WebUI.Models.Downloads
{
    public class SearchDownloadsModel : ListModel
    {
        [Display(Name = "Client name")]
        public string Name { get; set; }

        [Display(Name = "Publication name")]
        public string PublicationName { get; set; }

        [Display(Name = "Subject Matter")]
        public string SubjectMatter { get; set; }

        public string Status { get; set; }

        public int? ClientId { get; set; }

        public bool IsEmpty()
        {
            if (!String.IsNullOrEmpty(this.Name) || !String.IsNullOrEmpty(this.SubjectMatter) || !String.IsNullOrEmpty(this.PublicationName))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}