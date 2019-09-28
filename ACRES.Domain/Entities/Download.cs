using ACRES.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRES.Domain.Entities
{
   public class Download
    {

        public Download()
        {
            Date = UgandaDateTime.DateNow();
            Publications = new List<DownloadItem>();
        }

        [Key]
        public int DownloadId { get; set; }

        [StringLength(128)]
        [ForeignKey("Client")]
        public string ClientId { get; set; }

        public virtual ICollection<DownloadItem> Publications { get; set; }

        public DateTime Date { get; set; }

        public virtual ApplicationUser Client { get; set; }

        public bool Contains(string matter)
        {
            return Publications.Where(x => x.Publication.SubjectMatter.ToLower().Contains(matter)).Count() > 0;
        }

        public string GetClient()
        {
            return string.IsNullOrEmpty(ClientId) ? "Anonymous" : Client.FullName;
        }

    }
}
