using ACRES.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRES.Domain.Entities
{
    public class Inquiry
    {
        public Inquiry()
        {
            Date = UgandaDateTime.DateNow();
        }

        [Key]
        public int InquiryId { get; set; }

        public DateTime Date { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Company { get; set; }

        [StringLength(100)]
        public string Subject { get; set; }

        [StringLength(1000)]
        public string Message { get; set; }

    }
}
