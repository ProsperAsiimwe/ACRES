using ACRES.Domain.Context;
using ACRES.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ACRES.Domain.Concrete
{
    public class ActivityLogger
    {
        ApplicationDbContext context = new ApplicationDbContext();

        public void log(string userName, string usageKind, string usageDetails)
        {
            Log log = new Log();
            log.aluTime = DateTime.Now;
            log.userName = userName;
            log.userHost = HttpContext.Current.Request.Url.Host.ToString();
            log.userAddress = HttpContext.Current.Request.UserHostAddress.ToString();
            log.usageDetails = usageDetails;
            log.usageKind = usageKind;
            context.Log.Add(log);
            context.SaveChanges();
        }

    }
}
