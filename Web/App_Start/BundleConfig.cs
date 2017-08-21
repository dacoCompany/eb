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
               .Include("~/Scripts/autocomplete.js"));

            bundles.Add(new ScriptBundle("~/bundles/chosen")
               .Include("~/Scripts/chosen.jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/categories")
              .Include("~/Scripts/categories.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery_ui")
               .Include("~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery_addons")
               .Include("~/Scripts/jquery.validate.min.js")
               .Include("~/Scripts/respond.js")
               .Include("~/Scripts/modernizr-*")
               .Include("~/Scripts/bootstrap-hover-dropdown.js"));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/BaseContent/css/bootstrap.css")
                .Include("~/Content/BaseContent/css/bootstrap-theme.css")
                .Include("~/Content/BaseContent/css/style.css")
                .Include("~/Content/BaseContent/css/button.css")
                .Include("~/Content/BaseContent/css/jquery-ui.css")
                .Include("~/Content/BaseContent/css/reset.css")
                .Include("~/Content/BaseContent/css/font-awesome.min.css")
                .Include("~/Content/BaseContent/css/media-queries.css"));

            bundles.Add(new StyleBundle("~/Content/login")
                .Include("~/Content/BaseContent/css/login.css"));

            bundles.Add(new StyleBundle("~/Content/check-radio")
                .Include("~/Content/BaseContent/css/check_radio.css"));

            bundles.Add(new StyleBundle("~/Content/chosen")
                .Include("~/Content/BaseContent/css/chosen.css"));

            bundles.Add(new StyleBundle("~/Content/settings")
               .Include("~/Content/BaseContent/css/user-settings.css"));

            bundles.Add(new StyleBundle("~/Content/jQuery-File-Upload").Include(
                   "~/Content/jQuery.FileUpload/css/jquery.fileupload.css",
                   "~/Content/jQuery.FileUpload/css/jquery.fileupload-ui.css",
                   "~/Content/blueimp-gallery2/css/blueimp-gallery.css",
                   "~/Content/blueimp-gallery2/css/blueimp-gallery-video.css",
                   "~/Content/blueimp-gallery2/css/blueimp-gallery-indicator.css"
                   ));

            bundles.Add(new ScriptBundle("~/bundles/jQuery-File-Upload").Include(
                    "~/Scripts/jQuery.FileUpload/vendor/jquery.ui.widget.js",
                    "~/Scripts/jQuery.FileUpload/tmpl.min.js",
                    "~/Scripts/jQuery.FileUpload/load-image.all.min.js",
                    "~/Scripts/jQuery.FileUpload/canvas-to-blob.min.js",
                    "~/Scripts/jQuery.FileUpload/jquery.iframe-transport.js",
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload.js",
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-process.js",
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-image.js",
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-audio.js",
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-video.js",
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-validate.js",
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-ui.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-video.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-indicator.js",
                    "~/Scripts/blueimp-gallery2/js/jquery.blueimp-gallery.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/Blueimp-Gallerry2").Include(
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-video.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-indicator.js",
                    "~/Scripts/blueimp-gallery2/js/jquery.blueimp-gallery.js"));
        }
    }
}
