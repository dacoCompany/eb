using System.Web;
using System.Web.Optimization;

namespace Web.eBado
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/recaptcha", "//www.google.com/recaptcha/api.js").Include("~/Scripts/recaptcha-api.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery_base")
               .Include("~/Scripts/jquery-{version}.js")
               .Include("~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery_autocomplete")
               .Include("~/Scripts/autocomplete.js"));

            bundles.Add(new ScriptBundle("~/bundles/postalCode")
              .Include("~/Scripts/setPostalCode.js"));

            bundles.Add(new ScriptBundle("~/bundles/changeSettings")
             .Include("~/Scripts/changeSettings.js"));

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
                .Include("~/Content/BaseContent/css/bootstrap.css", new CssRewriteUrlTransform())
                .Include("~/Content/BaseContent/css/bootstrap-theme.css", new CssRewriteUrlTransform())
                .Include("~/Content/BaseContent/css/button.css", new CssRewriteUrlTransform())
                .Include("~/Content/BaseContent/css/jquery-ui.css", new CssRewriteUrlTransform())
                .Include("~/Content/BaseContent/css/reset.css", new CssRewriteUrlTransform())
                .Include("~/Content/BaseContent/css/font-awesome.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/BaseContent/css/media-queries.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/main_style")
                .Include("~/Content/BaseContent/css/style.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/site_area")
                .Include("~/Content/BaseContent/css/site-area.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/login")
                .Include("~/Content/BaseContent/css/login.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/error")
               .Include("~/Content/BaseContent/css/error.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/batch")
               .Include("~/Content/BaseContent/css/batch-gallery.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/check-radio")
                .Include("~/Content/BaseContent/css/check_radio.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/chosen")
                .Include("~/Content/BaseContent/css/chosen.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/company")
               .Include("~/Content/BaseContent/css/company.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/companyDetail")
              .Include("~/Content/BaseContent/css/company-detail.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/settings")
               .Include("~/Content/BaseContent/css/user-settings.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/jQuery-File-Upload").Include(
                   "~/Content/jQuery.FileUpload/css/jquery.fileupload.css",
                   "~/Content/jQuery.FileUpload/css/jquery.fileupload-ui.css",
                   "~/Content/blueimp-gallery2/css/blueimp-gallery.css",
                   "~/Content/blueimp-gallery2/css/blueimp-gallery-video.css",
                   "~/Content/blueimp-gallery2/css/blueimp-gallery-indicator.css",
                   "~/Content/BaseContent/css/youtube.css"
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
                    "~/Scripts/blueimp-gallery2/js/jquery.blueimp-gallery.js",
                    "~/Scripts/youtube.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/Blueimp-Gallerry2").Include(
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-video.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-indicator.js",
                    "~/Scripts/blueimp-gallery2/js/jquery.blueimp-gallery.js"));
        }
    }
}
