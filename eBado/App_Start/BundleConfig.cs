using System.Web;
using System.Web.Optimization;

namespace Web.eBado
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery_base")
               .Include("~/Scripts/jquery-{version}.js")
               .Include("~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery_autocomplete")
                .Include("~/Scripts/jquery-ui-{version}.js")
               .Include("~/Scripts/autocomplete.js"));

            bundles.Add(new ScriptBundle("~/bundles/categories")
              .Include("~/Scripts/categories.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery_addons")
               .Include("~/Scripts/jquery.validate.min.js")
               .Include("~/Scripts/respond.js")
               .Include("~/Scripts/modernizr-*")
               .Include("~/Scripts/bootstrap-hover-dropdown.js"));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Assets/css/bootstrap.css")
                .Include("~/Assets/css/bootstrap-theme.css")
                .Include("~/Assets/css/style.css")
                .Include("~/Assets/css/button.css")
                .Include("~/Assets/css/reset.css")
                .Include("~/Assets/css/font-awesome.min.css")
                .Include("~/Assets/css/media-queries.css"));

            bundles.Add(new StyleBundle("~/Content/login")
                .Include("~/Assets/css/login.css"));

            bundles.Add(new StyleBundle("~/Content/jquery_ui")
                .Include("~/Assets/css/jquery-ui.css"));

            bundles.Add(new StyleBundle("~/Content/settings")
               .Include("~/Assets/css/user-settings.css"));
        }
    }
}
