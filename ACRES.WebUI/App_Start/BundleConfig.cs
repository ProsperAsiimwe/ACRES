using System.Web;
using System.Web.Optimization;

namespace ACRES.WebUI
{
    public class BundleConfig
    {

        public static void RegisterBundles(BundleCollection bundles)
        {
            
            bundles.Add(new StyleBundle("~/Content/styles/core").Include(
                     "~/Content/coreAssets/css/bootstrap.min.css",
                     "~/Content/coreAssets/css/font-awesome.min.css",
                     "~/Content/coreAssets/css/material-design-iconic-font.min.css",
                     "~/Content/coreAssets/css/plugin.css",
                     "~/Content/coreAssets/style.css",
                     "~/Content/coreAssets/css/responsive.css"));

            bundles.Add(new ScriptBundle("~/bundles/headscripts").Include(
                      "~/Content/coreAssets/js/vendor/modernizr-2.8.3.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/sitescripts").Include(
                       "~/Content/coreAssets/js/vendor/jquery-1.12.4.min.js",
                       "~/Content/coreAssets/js/popper.min.js",
                       "~/Content/coreAssets/js/bootstrap.min.js",
                       "~/Content/coreAssets/js/ajax-mail.js",
                        "~/Content/coreAssets/js/plugins.js",
                       "~/Content/coreAssets/js/main.js"));
        }
    }
}

