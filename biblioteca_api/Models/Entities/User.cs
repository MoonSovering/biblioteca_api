using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using biblioteca_api.Interfaces;
using System.Collections.Generic;

namespace biblioteca_api.Models.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; }

        [Required]
        [JsonIgnore]
        [Column(TypeName = "varchar(255)")]
        public string PasswordHash { get; set; }

        [NotMapped]
        [JsonIgnore]
        public string Password { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        [Required]
        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string UserType { get; set; } // "Admin", "Reader", "Assistant", "Supplier"

        // Relaciones
        [JsonIgnore]
        public ICollection<Loan> Loans { get; set; }

        [JsonIgnore]
        public ICollection<Reservation> Reservations { get; set; }

        // Métodos de autenticación
        public void SetPassword(string password, IPasswordHasher passwordHasher)
        {
            PasswordHash = passwordHasher.HashPassword(password);
        }

        public bool VerifyPassword(string password, IPasswordHasher passwordHasher)
        {
            return passwordHasher.VerifyHashedPassword(PasswordHash, password);
        }

        public bool IsAdmin() => UserType == "Admin";
        public bool IsReader() => UserType == "Reader";
        public bool IsAssistant() => UserType == "Assistant";
        public bool IsSupplier() => UserType == "Supplier";

        public bool CanBorrowBooks(int maxLoans = 5)
        {
            if (!IsReader()) return false;

            var activeLoansCount = Loans?.Count(l => l.Status == "Active");
            return activeLoansCount < maxLoans;
        }

        public bool HasOverdueLoans()
        {
            return Loans?.Any(l => l.IsOverdue()) ?? false;
        }

        public int AccessLevel()
        {
            return UserType switch
            {
                "Admin" => 4,
                "Assistant" => 3,
                "Supplier" => 2,
                "Reader" => 1,
                _ => 0
            };
        }
    }
}