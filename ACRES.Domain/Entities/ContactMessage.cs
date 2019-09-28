using ACRES.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRES.Domain.Entities
{
   public class ContactMessage
    {

        public ContactMessage()
        {
            Date = UgandaDateTime.DateNow();
        }

        [Key]
        public int ContactMessageId { get; set; }

        [Display(Name = "Name")]
        [StringLength(50)]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [StringLength(50)]
        public string Email { get; set; }

        [Display(Name = "Subject")]
        [StringLength(50)]
        public string Subject { get; set; }

        [Display(Name = "Phone")]
        [StringLength(50)]
        public string Phone { get; set; }

        [Display(Name = "Message")]
        [StringLength(2000)]
        public string Message { get; set; }

        public DateTime Date { get; set; }

    }
}
