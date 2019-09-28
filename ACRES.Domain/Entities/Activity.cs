using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRES.Domain.Entities
{
    public class Activity
    {
        public Activity()
        {
            Recorded = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityId { get; set; }

        [ForeignKey("User")]
        [StringLength(128)]
        public string UserId { get; set; }

        public int? ArticleId { get; set; }

        public int? NewsletterId { get; set; }

        public int? MembershipId { get; set; }

        public int? PublicationId { get; set; }

        public int? DownloadId { get; set; }

        [StringLength(128)]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime Recorded { get; set; }

        [ForeignKey("RecordedBy")]
        [StringLength(128)]
        public string RecordedById { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ApplicationUser RecordedBy { get; set; }
    }
}
