using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace biblioteca_api.Models.Entities
{
    [Table("Books")]
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título del libro es obligatorio")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "El título debe tener entre 1 y 200 caracteres")]
        [Column("Title", TypeName = "varchar(200)")]
        public string Title { get; set; }

        [Required(ErrorMessage = "El autor es obligatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "El nombre del autor debe tener entre 1 y 100 caracteres")]
        [Column("Author", TypeName = "varchar(100)")]
        public string Author { get; set; }

        public int? AuthorEntityId { get; set; }
        [ForeignKey("AuthorEntityId")]
        public Author? AuthorEntity { get; set; }

        public int PublisherId { get; set; }
        [ForeignKey("PublisherId")]
        public Publisher Publisher { get; set; }

        [Column("PublishedAt", TypeName = "datetime2")]
        public DateTime? PublishedAt { get; set; }

        [Column("Price", TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "El ISBN es obligatorio")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "El ISBN debe tener entre 10 y 20 caracteres")]
        [Column("ISBN", TypeName = "varchar(20)")]
        public string ISBN { get; set; }

        [Column("AvailableCopies")]
        public int AvailableCopies { get; set; } = 1;

        [Column("TotalCopies")]
        public int TotalCopies { get; set; } = 1;

        public int SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }

        public ICollection<Loan> Loans { get; set; }
        public ICollection<Reservation> Reservations { get; set; }

        public bool IsAvailable() => AvailableCopies > 0;

        public void BorrowCopy()
        {
            if (AvailableCopies <= 0)
                throw new InvalidOperationException("No hay copias disponibles");
            AvailableCopies--;
        }

        public void ReturnCopy()
        {
            if (AvailableCopies >= TotalCopies)
                throw new InvalidOperationException("No se pueden devolver más copias de las existentes");
            AvailableCopies++;
        }

        public void UpdateAvailability(int change)
        {
            var newAvailability = AvailableCopies + change;
            if (newAvailability < 0 || newAvailability > TotalCopies)
                throw new InvalidOperationException("Cantidad de copias no válida");

            AvailableCopies = newAvailability;
        }
    }
}