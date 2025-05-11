using System.ComponentModel.DataAnnotations;

namespace biblioteca_api.Models.DTOs.Author
{
    public class CreateAuthorDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        public string Nationality { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Biography { get; set; }
    }
}