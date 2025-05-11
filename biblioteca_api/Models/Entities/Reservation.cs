using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace biblioteca_api.Models.Entities
{
    [Table("Reservations")]
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("BookId")]
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        [Column("ReservationDate", TypeName = "datetime2")]
        public DateTime ReservationDate { get; set; } = DateTime.UtcNow;

        [Column("ExpirationDate", TypeName = "datetime2")]
        public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(2);

        [Column("Status", TypeName = "varchar(20)")]
        public string Status { get; set; } = "Pending";

        public bool IsExpired() => DateTime.UtcNow > ExpirationDate && Status == "Pending";

        public void Complete()
        {
            Status = "Completed";
        }

        public void Cancel()
        {
            Status = "Cancelled";
        }
    }
}