namespace biblioteca_api.Models.DTOs.Publisher
{
    public class PublisherDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime FoundationDate { get; set; }
    }
}