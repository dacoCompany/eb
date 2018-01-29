namespace Infrastructure.Common.Models
{
    public class CachedLocationsModel
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string PostalCode{ get; set; }
        public string City{ get; set; }
        public string CityAlias{ get; set; }
        public string County{ get; set; }
        public string District{ get; set; }
        public string DistrictAlias { get; set; }
        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
    }
}