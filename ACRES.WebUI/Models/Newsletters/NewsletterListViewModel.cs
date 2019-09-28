using ACRES.Domain.Entities;
using MagicApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACRES.WebUI.Models.Newsletters
{
    public class NewsletterListViewModel
    {
        public IEnumerable<Newsletter> Newsletters { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public SearchNewslettersModel SearchModel { get; set; }

        public string[] Roles { get; set; }
    }
}