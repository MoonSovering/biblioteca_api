using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace biblioteca_api.Models.Entities
{
    [Table("Publishers")]
    public class Publisher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la editorial es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        [Column("Name", TypeName = "varchar(100)")]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
        [Column("Address", TypeName = "varchar(200)")]
        public string Address { get; set; }

        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [Column("Phone", TypeName = "varchar(20)")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Column("Email", TypeName = "varchar(100)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La fecha de fundación es obligatoria")]
        [Column("FoundationDate", TypeName = "datetime2")]
        public DateTime FoundationDate { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}