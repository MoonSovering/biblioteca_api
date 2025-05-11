namespace biblioteca_api.Models.Entities
{
    public class Supplier : User
    {
        public string CompanyName { get; set; }
        public string TaxId { get; set; }
        public ICollection<Book> SuppliedBooks { get; set; }
    }
}