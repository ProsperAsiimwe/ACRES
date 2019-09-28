using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRES.Domain.Entities
{
   public class VolunteerRequest
    {
        [Key]
        public int VolunteerRequestId { get; set; }

    }
}
