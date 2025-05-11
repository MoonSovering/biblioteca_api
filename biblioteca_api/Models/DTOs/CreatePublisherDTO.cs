using System.ComponentModel.DataAnnotations;

namespace biblioteca_api.Models.DTOs.Publisher
{
    public class CreatePublisherDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        public string Address { get; set; }

        [Phone]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime FoundationDate { get; set; }
    }
}