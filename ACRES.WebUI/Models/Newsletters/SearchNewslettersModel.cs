using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACRES.WebUI.Models.Newsletters
{
    public class SearchNewslettersModel : ListModel
    {

        [Display(Name = "Letter title")]
        public string NewsletterName { get; set; }


        [Display(Name = "Newsletter Id")]
        public int? NewsletterId { get; set; }

        public string Status { get; set; }


        public bool IsEmpty()
        {
            if (!String.IsNullOrEmpty(this.NewsletterName) || NewsletterId.HasValue)
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