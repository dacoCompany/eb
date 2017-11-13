using System;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Web.eBado
{
    public class JsonNetResult : JsonResult
    {
        public new Encoding ContentEncoding { get; set; }
        public new string ContentType { get; set; }
        public new object Data { get; set; }

        public JsonSerializerSettings SerializerSettings { get; set; }
        public Formatting Formatting { get; set; }

        private int statusCode { get; }
        private string description { get; }

        public JsonNetResult(object data, Formatting formatting, HttpStatusCode statusCode = HttpStatusCode.OK, string description = "")
            : this(data)
        {
            Formatting = formatting;
            this.statusCode = Convert.ToInt32(statusCode);
            this.description = string.IsNullOrEmpty(description) ? HttpWorkerRequest.GetStatusDescription(this.statusCode) : description;
        }

        public JsonNetResult(object data, HttpStatusCode statusCode = HttpStatusCode.OK, string description = "") : this()
        {
            Data = data;
            this.statusCode = Convert.ToInt32(statusCode);
            this.description = string.IsNullOrEmpty(description) ? HttpWorkerRequest.GetStatusDescription(this.statusCode) : description;
        }

        public JsonNetResult(HttpStatusCode statusCode = HttpStatusCode.OK, string description = "")
        {
            Formatting = Formatting.None;
            SerializerSettings = new JsonSerializerSettings();
            this.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            this.statusCode = Convert.ToInt32(statusCode);
            this.description = string.IsNullOrEmpty(description) ? HttpWorkerRequest.GetStatusDescription(this.statusCode) : description;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            response.StatusCode = statusCode;
            response.StatusDescription = description;

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data == null) return;

            var writer = new JsonTextWriter(response.Output) { Formatting = Formatting };
            var serializer = JsonSerializer.Create(SerializerSettings);
            serializer.Serialize(writer, Data);
            writer.Flush();
        }
    }
}