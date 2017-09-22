using System.Collections.Generic;

namespace Web.eBado.Models.Shared
{
    public class CompanySessionModel
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CompanyRole { get; set; }
        public IEnumerable<string> CompanyPermissions { get; set; }
    }
}