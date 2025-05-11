using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace biblioteca_api.Models.Entities
{
    [Table("Authors")]
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del autor es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        [Column("Name", TypeName = "varchar(100)")]
        public string Name { get; set; }

        [StringLength(50, ErrorMessage = "La nacionalidad no puede exceder 50 caracteres")]
        [Column("Nationality", TypeName = "varchar(50)")]
        public string Nationality { get; set; }

        [Column("BirthDate", TypeName = "datetime2")]
        public DateTime? BirthDate { get; set; }

        [Column("Biography", TypeName = "text")]
        public string Biography { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}