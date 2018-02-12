namespace Web.eBado.Models.Shared
{
    public class OfferingSiteAreaModel<T> : SiteAreaBaseModel<T> where T: OfferingItemModel
    {
        public int MyProperty { get; set; }
    }
}