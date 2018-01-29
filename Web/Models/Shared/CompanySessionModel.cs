using Infrastructure.Common.Enums;
using System.Collections.Generic;

namespace Web.eBado.Models.Shared
{
    public class CompanySessionModel
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsProfi { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CompanyRole { get; set; }
        public string CompanyType { get; set; }
        public string ProfileUrl { get; set; }
        public IEnumerable<string> CompanyPermissions { get; set; }
    }
}