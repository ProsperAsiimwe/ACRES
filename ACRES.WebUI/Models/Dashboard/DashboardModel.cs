using ACRES.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACRES.WebUI.Models.Dashboard
{
    public class DashboardModel
    {
        public IEnumerable<Activity> Activities { get; set; }

        public int? TermId { get; set; }

        public IEnumerable<Activity> GetLatestActivity()
        {
            return Activities
                .OrderByDescending(o => o.Recorded)
                .Take(10);
        }

      

    }
}