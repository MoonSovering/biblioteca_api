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

            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                throw new ApplicationException("El nombre de usuario ya existe");

            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                throw new ApplicationException("El email ya está registrado");

            user.SetPassword(password, _passwordHasher);
            user.RegisteredAt = DateTime.UtcNow;
            user.IsActive = true;
            user.UserType ??= "Reader";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task Update(User userParam, string password = null)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userParam.Id && u.IsActive)
                ?? throw new KeyNotFoundException("Usuario no encontrado");

            if (userParam.Username != user.Username &&
                await _context.Users.AnyAsync(u => u.Username == userParam.Username))
            {
                throw new ApplicationException("El nombre de usuario ya existe");
            }

            if (userParam.Email != user.Email &&
                await _context.Users.AnyAsync(u => u.Email == userParam.Email))
            {
                throw new ApplicationException("El email ya está registrado");
            }

            user.Username = userParam.Username;
            user.Email = userParam.Email;
            user.IsActive = userParam.IsActive;
            user.UserType = userParam.UserType;

            if (!string.IsNullOrWhiteSpace(password))
            {
                user.SetPassword(password, _passwordHasher);
            }

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsAdmin(int userId)
        {
            var user = await GetById(userId);
            return user.IsAdmin();
        }
    }
}