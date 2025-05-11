using biblioteca_api.Models.DTOs.Author;
using biblioteca_api.Models.DTOs.Publisher;
using biblioteca_api.Models.DTOs.Supplier;

namespace biblioteca_api.Models.DTOs.Book
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Author { get; set; }

        public AuthorSimpleDTO AuthorEntity { get; set; }

        public PublisherSimpleDTO Publisher { get; set; }

        public DateTime? PublishedAt { get; set; }
        public decimal? Price { get; set; }
        public string ISBN { get; set; }
        public int AvailableCopies { get; set; }
        public int TotalCopies { get; set; }

        public SupplierSimpleDTO Supplier { get; set; }
    }
}