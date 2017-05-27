using System.Web;
using System.Web.Optimization;

namespace Web.eBado
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
               .Include("~/Scripts/jquery-{version}.js")
               .Include("~/Scripts/jquery.validate.min.js")
               .Include("~/Scripts/bootstrap.js")
               .Include("~/Scripts/style.js")
               .Include("~/Scripts/respond.js")
               .Include("~/Scripts/modernizr-*")
               .Include("~/Scripts/bootstrap-hover-dropdown.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Assets/css/bootstrap.css",
                      "~/Assets/css/bootstrap-theme.css",
                      "~/Assets/css/style.css",
                      "~/Assets/css/button.css",
                      "~/Assets/css/reset.css",
                      "~/Assets/css/font-awesome.min.css",
                      "~/Assets/css/media-queries.css"));
        }
    }
}
