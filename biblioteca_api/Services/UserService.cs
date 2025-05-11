using biblioteca_api.Models.Entities;
using biblioteca_api.Data;
using biblioteca_api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;

namespace biblioteca_api.Services
{
    public class UserService : IUserService
    {
        private readonly LibraryContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(LibraryContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new ArgumentNullException("Usuario y contraseña son requeridos");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null || !user.VerifyPassword(password, _passwordHasher))
                throw new AuthenticationException("Usuario o contraseña incorrectos");

            user.UpdateLastLogin();
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive)
                ?? throw new KeyNotFoundException("Usuario no encontrado");
        }

        public async Task<User> Create(User user, string password)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña es requerida");

            if (_context.Users.Any(u => u.Username == user.Username))
                throw new ApplicationException("El nombre de usuario ya existe");

            if (_context.Users.Any(u => u.Email == user.Email))
                throw new ApplicationException("El email ya está registrado");

            // Hash password
            user.SetPassword(password, _passwordHasher);
            user.RegisteredAt = DateTime.UtcNow;
            user.IsActive = true;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task Update(User userParam, string password = null)
        {
            var user = await _context.Users.FindAsync(userParam.Id)
                ?? throw new KeyNotFoundException("Usuario no encontrado");

            // Validar unicidad de username
            if (userParam.Username != user.Username)
            {
                if (_context.Users.Any(u => u.Username == userParam.Username))
                    throw new ApplicationException("El nombre de usuario ya existe");
            }

            // Actualizar propiedades
            user.Username = userParam.Username;
            user.Email = userParam.Email;
            user.IsActive = userParam.IsActive;

            // Actualizar password si se proporcionó
            if (!string.IsNullOrWhiteSpace(password))
            {
                user.SetPassword(password, _passwordHasher);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                // Borrado lógico
                user.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AssignRole(int userId, int roleId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new KeyNotFoundException("Usuario no encontrado");

            var role = await _context.Roles.FindAsync(roleId)
                ?? throw new KeyNotFoundException("Rol no encontrado");

            if (user.UserRoles.Any(ur => ur.RoleId == roleId))
                throw new ApplicationException("El usuario ya tiene asignado este rol");

            user.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>> GetUserRoles(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.UserRoles.Select(ur => ur.Role.Name) ?? Enumerable.Empty<string>();
        }
    }
}