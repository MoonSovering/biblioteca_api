using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace biblioteca_api.Models.Entities
{
    [Table("Loans")]
    public class Loan
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
        [Column("LoanDate", TypeName = "datetime2")]
        public DateTime LoanDate { get; set; } = DateTime.UtcNow;

        [Column("DueDate", TypeName = "datetime2")]
        public DateTime DueDate { get; set; }

        [Column("ReturnDate", TypeName = "datetime2")]
        public DateTime? ReturnDate { get; set; }

        [Column("Status", TypeName = "varchar(20)")]
        public string Status { get; set; } = "Active";

        public bool IsOverdue() => DateTime.UtcNow > DueDate && Status != "Returned";

        public void MarkAsReturned()
        {
            ReturnDate = DateTime.UtcNow;
            Status = "Returned";
            Book?.ReturnCopy();
        }
    }
}