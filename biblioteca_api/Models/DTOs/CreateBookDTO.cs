using System.ComponentModel.DataAnnotations;

namespace biblioteca_api.Models.DTOs.Book
{
    public class CreateBookDTO
    {
        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; }

        public string? Author { get; set; }

        [Required(ErrorMessage = "El ID del autor es obligatorio")]
        public int AuthorEntityId { get; set; }

        [Required(ErrorMessage = "El editor es obligatorio")]
        public int PublisherId { get; set; }

        public DateTime? PublishedAt { get; set; }

        [Range(0, 10000000)]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "El ISBN es obligatorio")]
        [StringLength(20, MinimumLength = 10)]
        public string ISBN { get; set; }

        [Range(1, 100)]
        public int TotalCopies { get; set; } = 1;

        [Required(ErrorMessage = "El proveedor es obligatorio")]
        public int SupplierId { get; set; }
    }
}