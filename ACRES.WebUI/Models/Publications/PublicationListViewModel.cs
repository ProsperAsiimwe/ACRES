using ACRES.Domain.Entities;
using MagicApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACRES.WebUI.Models.Publications
{
    public class PublicationListViewModel
    {
        public IEnumerable<Publication> Publications { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public SearchPublicationsModel SearchModel { get; set; }

        public string[] Roles { get; set; }
    }
}