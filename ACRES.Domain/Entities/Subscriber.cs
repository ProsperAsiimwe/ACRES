using ACRES.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRES.Domain.Entities
{
    public class Subscriber
    {
        public Subscriber()
        {
            Added = UgandaDateTime.DateNow();
        }

        [Key]
        public int SubscriberId { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        public DateTime Added { get; set; }

        public DateTime? Cancelled { get; set; }

    }
}
