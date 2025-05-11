using biblioteca_api.Data;
using biblioteca_api.Interfaces;
using biblioteca_api.Models.DTOs.Author;
using biblioteca_api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace biblioteca_api.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly LibraryContext _context;

        public AuthorService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuthorDTO>> GetAll()
        {
            return await _context.Authors
                .OrderBy(a => a.Name)
                .Select(a => new AuthorDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Nationality = a.Nationality,
                    BirthDate = a.BirthDate,
                    Biography = a.Biography
                })
                .ToListAsync();
        }

        public async Task<AuthorDTO> GetById(int id)
        {
            var author = await _context.Authors
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new KeyNotFoundException("Autor no encontrado");

            return new AuthorDTO
            {
                Id = author.Id,
                Name = author.Name,
                Nationality = author.Nationality,
                BirthDate = author.BirthDate,
                Biography = author.Biography
            };
        }

        public async Task<AuthorDTO> Create(CreateAuthorDTO authorDto)
        {
            var author = new Author
            {
                Name = authorDto.Name,
                Nationality = authorDto.Nationality,
                BirthDate = authorDto.BirthDate,
                Biography = authorDto.Biography
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return new AuthorDTO
            {
                Id = author.Id,
                Name = author.Name,
                Nationality = author.Nationality,
                BirthDate = author.BirthDate,
                Biography = author.Biography
            };
        }

        public async Task Update(int id, CreateAuthorDTO authorDto)
        {
            var existingAuthor = await _context.Authors.FindAsync(id)
                ?? throw new KeyNotFoundException("Autor no encontrado");

            existingAuthor.Name = authorDto.Name;
            existingAuthor.Nationality = authorDto.Nationality;
            existingAuthor.BirthDate = authorDto.BirthDate;
            existingAuthor.Biography = authorDto.Biography;

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null) return;

            if (author.Books.Any())
                throw new InvalidOperationException("No se puede eliminar un autor que tiene libros asociados");

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuthorDTO>> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await GetAll();

            query = query.ToLower();
            return await _context.Authors
                .Where(a => a.Name.ToLower().Contains(query) ||
                          a.Nationality.ToLower().Contains(query))
                .Select(a => new AuthorDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Nationality = a.Nationality,
                    BirthDate = a.BirthDate,
                    Biography = a.Biography
                })
                .ToListAsync();
        }
    }
}