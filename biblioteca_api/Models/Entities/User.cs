using biblioteca_api.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.PortableExecutable;
using System.Text.Json.Serialization;

namespace biblioteca_api.Models.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres")]
        [Column("Username", TypeName = "varchar(50)")]
        public string Username { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        [Column("Email", TypeName = "varchar(100)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [JsonIgnore]
        [Column("PasswordHash", TypeName = "varchar(255)")]
        public string PasswordHash { get; set; }

        [NotMapped]
        public string Password { get; set; }

        [Column("RefreshToken", TypeName = "varchar(255)")]
        public string RefreshToken { get; set; }

        [Column("RegisteredAt", TypeName = "datetime2")]
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        [Column("LastLogin", TypeName = "datetime2")]
        public DateTime? LastLogin { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("UserType", TypeName = "varchar(20)")]
        public string UserType { get; set; }

        // Relaciones
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        // Métodos
        public void SetPassword(string password, IPasswordHasher passwordHasher)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña no puede estar vacía", nameof(password));

            PasswordHash = passwordHasher.HashPassword(password);
        }

        public bool VerifyPassword(string password, IPasswordHasher passwordHasher)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(PasswordHash))
                return false;

            return passwordHasher.VerifyHashedPassword(PasswordHash, password);
        }

        public void UpdateLastLogin()
        {
            LastLogin = DateTime.UtcNow;
        }

        // Métodos de herencia
        public bool IsAdmin() => UserType == nameof(Admin);
        public bool IsReader() => UserType == nameof(Reader);
        public bool IsAssistant() => UserType == nameof(Assistant);
        public bool IsSupplier() => UserType == nameof(Supplier);
    }
}