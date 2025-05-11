using System.ComponentModel.DataAnnotations;

namespace biblioteca_api.Models.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime? PublishedAt { get; set; }
        public decimal? Price { get; set; }
        public string ISBN { get; set; }
        public int AvailableCopies { get; set; }
        public int TotalCopies { get; set; }
        public int SupplierId { get; set; }
    }

    public class CreateBookDTO
    {
        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; }

        [Required(ErrorMessage = "El autor es obligatorio")]
        [StringLength(100, MinimumLength = 1)]
        public string Author { get; set; }

        public DateTime? PublishedAt { get; set; }

        [Range(0, 1000)]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "El ISBN es obligatorio")]
        [StringLength(20, MinimumLength = 10)]
        public string ISBN { get; set; }

        [Range(1, 100)]
        public int TotalCopies { get; set; } = 1;

        [Required(ErrorMessage = "El proveedor es obligatorio")]
        public int SupplierId { get; set; }
    }

    public class UpdateBookDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Author { get; set; }

        public DateTime? PublishedAt { get; set; }

        [Range(0, 1000)]
        public decimal? Price { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 10)]
        public string ISBN { get; set; }

        [Range(1, 100)]
        public int TotalCopies { get; set; }
    }

    public class BookSearchResultDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int AvailableCopies { get; set; }
        public string Status { get; set; } // "Disponible", "Agotado", "Reservado"
    }
}