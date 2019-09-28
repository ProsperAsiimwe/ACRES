using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MagicApps.Infrastructure.Helpers;

namespace ACRES.WebUI.Infrastructure.Helpers
{
    public class CookieHelper
    {
        public void SetCookies(int referenceId)
        {
            ReferenceId = referenceId;
            Authorised = true;
        }

        public bool Authorised {
            get {
                bool _id = bool.Parse(CustomHelper.GetCookieValue("ACRES-Authorised", Boolean.FalseString));
                return _id;
            }
            set {
                CustomHelper.CreateCookie("ACRES-Authorised", value.ToString());
            }
        }

        public int ReferenceId {
            get {
                int _id = int.Parse(CustomHelper.GetCookieValue("ACRES-ReferenceId"));
                return _id;
            }
            set {
                CustomHelper.CreateCookie("ACRES-ReferenceId", value.ToString());
            }
        }

        public void Flush()
        {
            CustomHelper.CreateCookie("ACRES-Authorised", Boolean.FalseString, -1);
            CustomHelper.CreateCookie("ACRES-ReferenceId", "0", -1);
        }
    }
}