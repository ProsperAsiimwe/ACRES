using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRES.Domain.Entities
{
    public class Log
    {
        [Key]
        public int id { get; set; }
        public string userName { get; set; }
        public string usageKind { get; set; }
        public string usageDetails { get; set; }
        public string userAddress { get; set; }
        public string userHost { get; set; }
        public DateTime aluTime { get; set; }
    }
}
