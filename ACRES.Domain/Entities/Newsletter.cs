using ACRES.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRES.Domain.Entities
{
    public class Newsletter
    {
        public Newsletter()
        {
            Added = UgandaDateTime.DateNow();
        }

        [Key]
        public int NewsletterId { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(5000)]
        public string News { get; set; }

        public DateTime Added { get; set; }
    }
}
