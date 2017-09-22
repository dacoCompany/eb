using System;
using System.Collections.Generic;
using System.Linq;
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

        public JsonNetResult(object data, Formatting formatting)
            : this(data)
        {
            Formatting = formatting;
        }

        public JsonNetResult(object data) : this()
        {
            Data = data;
        }

        public JsonNetResult()
        {
            Formatting = Formatting.None;
            SerializerSettings = new JsonSerializerSettings();
            this.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
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