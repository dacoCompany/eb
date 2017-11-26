using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infrastructure.Common.Models
{
    public class CachedLanguagesModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string LanguageName{ get; set; }
    }
}