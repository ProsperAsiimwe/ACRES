using ACRES.Domain.Entities;
using MagicApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACRES.WebUI.Models.Downloads
{
    public class DownloadListViewModel
    {
        public IEnumerable<Download> Downloads { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public SearchDownloadsModel SearchModel { get; set; }

        public string[] Roles { get; set; }
    }
}