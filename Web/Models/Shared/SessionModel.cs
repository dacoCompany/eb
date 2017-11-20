using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Web.eBado.Models.Shared
{
    public class SessionModel
    {
        public SessionModel()
        {
            Companies = new Collection<CompanySessionModel>();
        }
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public bool IsExternalLogin { get; set; }
        public IEnumerable<string> UserPermissions { get; set; }
        public bool HasCompany { get; set; }
        public ICollection<CompanySessionModel> Companies { get; set; }

    }
}