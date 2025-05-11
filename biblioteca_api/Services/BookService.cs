using biblioteca_api.Models.Entities;
using biblioteca_api.Data;
using Microsoft.EntityFrameworkCore;
using biblioteca_api.Interfaces;
using biblioteca_api.Models.DTOs;
using biblioteca_api.Helpers.Mappers;

namespace biblioteca_api.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryContext _context;

        public BookService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookDTO>> GetAll()
        {
            return await _context.Books
                .Include(b => b.Supplier)
                .Where(b => b.TotalCopies > 0)
                .OrderBy(b => b.Title)
                .Select(b => MapToDTOHelper.MapToDTO(b))
                .ToListAsync();
        }

        public async Task<BookDTO> GetById(int id)
        {
            var book = await _context.Books
                .Include(b => b.Supplier)
                .FirstOrDefaultAsync(b => b.Id == id)
                ?? throw new KeyNotFoundException("Libro no encontrado");

            return MapToDTOHelper.MapToDTO(book);
        }

        public async Task<BookDTO> Create(CreateBookDTO bookDto)
        {
            if (await _context.Books.AnyAsync(b => b.ISBN == bookDto.ISBN))
                throw new InvalidOperationException("Ya existe un libro con este ISBN");

            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                PublishedAt = bookDto.PublishedAt,
                Price = bookDto.Price,
                ISBN = bookDto.ISBN,
                TotalCopies = bookDto.TotalCopies,
                AvailableCopies = bookDto.TotalCopies,
                SupplierId = bookDto.SupplierId
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return MapToDTOHelper.MapToDTO(book);
        }

        public async Task Update(UpdateBookDTO bookDto)
        {
            var existingBook = await _context.Books.FindAsync(bookDto.Id)
                ?? throw new KeyNotFoundException("Libro no encontrado");

            var borrowedCopies = existingBook.TotalCopies - existingBook.AvailableCopies;
            if (bookDto.TotalCopies < borrowedCopies)
                throw new InvalidOperationException($"No puedes reducir las copias totales por debajo de {borrowedCopies} (copias prestadas)");

            existingBook.Title = bookDto.Title;
            existingBook.Author = bookDto.Author;
            existingBook.PublishedAt = bookDto.PublishedAt;
            existingBook.Price = bookDto.Price;
            existingBook.ISBN = bookDto.ISBN;
            existingBook.AvailableCopies += (bookDto.TotalCopies - existingBook.TotalCopies);
            existingBook.TotalCopies = bookDto.TotalCopies;

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var book = await _context.Books
                .Include(b => b.Loans)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return;

            if (book.Loans.Any(l => l.Status == "Active"))
                throw new InvalidOperationException("No se puede eliminar un libro con préstamos activos");

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BookDTO>> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await GetAll();

            query = query.ToLower();
            return await _context.Books
                .Include(b => b.Supplier)
                .Where(b => b.Title.ToLower().Contains(query) ||
                           b.Author.ToLower().Contains(query) ||
                           b.ISBN.ToLower().Contains(query))
                .Select(b => MapToDTOHelper.MapToDTO(b))
                .ToListAsync();
        }

        public async Task<IEnumerable<BookDTO>> GetAvailableBooks()
        {
            return await _context.Books
                .Include(b => b.Supplier)
                .Where(b => b.AvailableCopies > 0)
                .OrderBy(b => b.Title)
                .Select(b => MapToDTOHelper.MapToDTO(b))
                .ToListAsync();
        }

        public async Task<IEnumerable<BookDTO>> GetBooksByAuthor(string author)
        {
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("El nombre del autor no puede estar vacío");

            return await _context.Books
                .Include(b => b.Supplier)
                .Where(b => b.Author.ToLower().Contains(author.ToLower()))
                .OrderBy(b => b.Title)
                .Select(b => MapToDTOHelper.MapToDTO(b))
                .ToListAsync();
        }

        public async Task<IEnumerable<BookDTO>> GetBooksByPublicationYear(int year)
        {
            if (year < 1000 || year > DateTime.Now.Year)
                throw new ArgumentException($"El año debe estar entre 1000 y {DateTime.Now.Year}");

            return await _context.Books
                .Include(b => b.Supplier)
                .Where(b => b.PublishedAt.HasValue && b.PublishedAt.Value.Year == year)
                .OrderBy(b => b.Title)
                .Select(b => MapToDTOHelper.MapToDTO(b))
                .ToListAsync();
        }

        public async Task<int> GetTotalBookCount()
        {
            return await _context.Books.CountAsync();
        }

        public async Task<int> GetAvailableCopiesCount(int bookId)
        {
            return await _context.Books
                .Where(b => b.Id == bookId)
                .Select(b => b.AvailableCopies)
                .FirstOrDefaultAsync();
        }
    }
}