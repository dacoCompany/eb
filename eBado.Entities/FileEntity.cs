using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBado.Entities
{
    public class FileEntity
    {
        public string Name { get; set; }

        public int Size { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        public string Url { get; set; }

        public string DeleteUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        public string DeleteType { get; set; }
    }
}
