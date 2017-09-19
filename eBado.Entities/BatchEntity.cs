using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBado.Entities
{
    public class BatchEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BaseThumbUrl { get; set; }
        public int AttachmentsCount { get; set; }
    }
}
