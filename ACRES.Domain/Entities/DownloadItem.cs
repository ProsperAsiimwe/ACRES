using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRES.Domain.Entities
{
    public class DownloadItem
    {

        public DownloadItem()
        {

        }

        public DownloadItem(int PublicationId, int DownloadId)
        {
            this.PublicationId = PublicationId;
            this.DownloadId = DownloadId;
        }

        [Key, ForeignKey("Publication"), Column(Order = 1)]
        public int PublicationId { get; set; }

        [Key, ForeignKey("Download"), Column(Order = 2)]
        public int DownloadId { get; set; }

        public virtual Download Download { get; set; }

        public virtual Publication Publication { get; set; }
    }
}
