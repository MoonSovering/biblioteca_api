using System.ComponentModel.DataAnnotations;

namespace biblioteca_api.Models.DTOs.Book
{
    public class UpdateBookDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; }

        public string? Author { get; set; }

        [Required]
        public int AuthorEntityId { get; set; }

        [Required]
        public int PublisherId { get; set; }

        public DateTime? PublishedAt { get; set; }

        [Range(0, 10000000)]
        public decimal? Price { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 10)]
        public string ISBN { get; set; }

        [Range(1, 100)]
        public int TotalCopies { get; set; }
    }
}