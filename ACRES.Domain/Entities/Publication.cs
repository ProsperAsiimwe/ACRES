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
   public class Publication
    {
        public Publication()
        {
            Submitted = UgandaDateTime.DateNow();
            Downloads = new List<DownloadItem>();
        }

        [Key]
        public int PublicationId { get; set; }

        public int Type { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(100)]
        public string SubjectMatter { get; set; }

        [StringLength(100)]
        public string Doc { get; set; }

        [Display(Name = "Split Page Preview")]
        public int SplitPage { get; set; }

        public DateTime Submitted { get; set; }

        public DateTime? Deleted { get; set; }

        public virtual ICollection<DownloadItem> Downloads { get; set; }

        public string GetDoc()
        {
            string docRoot = System.Configuration.ConfigurationManager.AppSettings["Settings.Site.DocFolder"];
            string docName = String.Format(@"ACRES_Publication_{0}.pdf", PublicationId);
            return string.Format(@"{0}\Publications\{1}", docRoot, docName);
        }

        public string GetDocPreview()
        {
            string docRoot = string.Format(@"{0}/Content/uploads/docs", System.Configuration.ConfigurationManager.AppSettings["Settings.Company.Url"]);
            string docName = String.Format(@"PTC_Publication_{0}-1.pdf", PublicationId);
            return string.Format(@"{0}/{1}", docRoot, docName);
        }

        public IDictionary<int, string> Types()
        {
            return new Dictionary<int, string>
            {
                {1, "Tax Publication" },
                {2, "Case Publication" }
            };
        }

        [NotMapped]
        public string TypeValue
        {
            get
            {
                return Types()[Type];
            }
        }
    }
}
