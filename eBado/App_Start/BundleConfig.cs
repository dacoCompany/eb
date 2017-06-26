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

            bundles.Add(new ScriptBundle("~/bundles/categories")
              .Include("~/Scripts/categories.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery_addons")
               .Include("~/Scripts/jquery-ui-{version}.js")
               .Include("~/Scripts/jquery.validate.min.js")
               .Include("~/Scripts/respond.js")
               .Include("~/Scripts/modernizr-*")
               .Include("~/Scripts/bootstrap-hover-dropdown.js"));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Assets/css/bootstrap.css")
                .Include("~/Assets/css/bootstrap-theme.css")
                .Include("~/Assets/css/style.css")
                .Include("~/Assets/css/button.css")
                .Include("~/Assets/css/jquery-ui.css")
                .Include("~/Assets/css/reset.css")
                .Include("~/Assets/css/font-awesome.min.css")
                .Include("~/Assets/css/media-queries.css"));

            bundles.Add(new StyleBundle("~/Content/login")
                .Include("~/Assets/css/login.css"));

            bundles.Add(new StyleBundle("~/Content/settings")
               .Include("~/Assets/css/user-settings.css"));

            bundles.Add(new StyleBundle("~/Content/jQuery-File-Upload").Include(
                   "~/Content/jQuery.FileUpload/css/jquery.fileupload.css",
                   "~/Content/jQuery.FileUpload/css/jquery.fileupload-ui.css",
                   "~/Content/blueimp-gallery2/css/blueimp-gallery.css",
                   "~/Content/blueimp-gallery2/css/blueimp-gallery-video.css",
                   "~/Content/blueimp-gallery2/css/blueimp-gallery-indicator.css"
                   ));

            bundles.Add(new ScriptBundle("~/bundles/jQuery-File-Upload").Include(
                    //<!-- The Templates plugin is included to render the upload/download listings -->
                    "~/Scripts/jQuery.FileUpload/vendor/jquery.ui.widget.js",
                    "~/Scripts/jQuery.FileUpload/tmpl.min.js",
                    //<!-- The Load Image plugin is included for the preview images and image resizing functionality -->
                    "~/Scripts/jQuery.FileUpload/load-image.all.min.js",
                    //<!-- The Canvas to Blob plugin is included for image resizing functionality -->
                    "~/Scripts/jQuery.FileUpload/canvas-to-blob.min.js",
                    //"~/Scripts/file-upload/jquery.blueimp-gallery.min.js",
                    //<!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
                    "~/Scripts/jQuery.FileUpload/jquery.iframe-transport.js",
                    //<!-- The basic File Upload plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload.js",
                    //<!-- The File Upload processing plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-process.js",
                    //<!-- The File Upload image preview & resize plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-image.js",
                    //<!-- The File Upload audio preview plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-audio.js",
                    //<!-- The File Upload video preview plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-video.js",
                    //<!-- The File Upload validation plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-validate.js",
                    //!-- The File Upload user interface plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-ui.js",
                    //Blueimp Gallery 2 
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-video.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-indicator.js",
                    "~/Scripts/blueimp-gallery2/js/jquery.blueimp-gallery.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/Blueimp-Gallerry2").Include(//Blueimp Gallery 2 
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-video.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-indicator.js",
                    "~/Scripts/blueimp-gallery2/js/jquery.blueimp-gallery.js"));
        }
    }
}
