using System.ComponentModel.DataAnnotations;

namespace biblioteca_api.Models.DTOs
{
    public class SupplierDTO
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string TaxId { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegisteredAt { get; set; }
        public int SuppliedBooksCount { get; set; }
    }

    public class CreateSupplierDTO
    {
        [Required(ErrorMessage = "El nombre de la compañía es obligatorio")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "El Tax ID es obligatorio")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        public string TaxId { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string Password { get; set; }
    }

    public class UpdateSupplierDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(20)]
        public string TaxId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool IsActive { get; set; }
    }

    public class SupplierBookDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int AvailableCopies { get; set; }
        public int TotalLoans { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class SupplierStatsDTO
    {
        public int TotalBooksSupplied { get; set; }
        public int ActiveLoans { get; set; }
        public int OverdueLoans { get; set; }
        public decimal TotalRevenue { get; set; }
        public Dictionary<string, int> LoansLastSixMonths { get; set; }
    }
}