using biblioteca_api.Models.Entities;
using biblioteca_api.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using biblioteca_api.Interfaces;

namespace biblioteca_api.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryContext _context;

        public BookService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAll()
        {
            return await _context.Books
                .Include(b => b.Supplier)
                .Where(b => b.TotalCopies > 0)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<Book> GetById(int id)
        {
            return await _context.Books
                .Include(b => b.Supplier)
                .FirstOrDefaultAsync(b => b.Id == id)
                ?? throw new KeyNotFoundException("Libro no encontrado");
        }

        public async Task<Book> Create(Book book)
        {
            if (await _context.Books.AnyAsync(b => b.ISBN == book.ISBN))
                throw new InvalidOperationException("Ya existe un libro con este ISBN");

            book.AvailableCopies = book.TotalCopies; 
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task Update(Book book)
        {
            var existingBook = await _context.Books.FindAsync(book.Id)
                ?? throw new KeyNotFoundException("Libro no encontrado");

            var borrowedCopies = existingBook.TotalCopies - existingBook.AvailableCopies;
            if (book.TotalCopies < borrowedCopies)
                throw new InvalidOperationException($"No puedes reducir las copias totales por debajo de {borrowedCopies} (copias prestadas)");

            
            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.PublishedAt = book.PublishedAt;
            existingBook.Price = book.Price;
            existingBook.ISBN = book.ISBN;

            
            existingBook.AvailableCopies += (book.TotalCopies - existingBook.TotalCopies);
            existingBook.TotalCopies = book.TotalCopies;

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

        public async Task<IEnumerable<Book>> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await GetAll();

            query = query.ToLower();
            return await _context.Books
                .Include(b => b.Supplier)
                .Where(b => b.Title.ToLower().Contains(query) ||
                           b.Author.ToLower().Contains(query) ||
                           b.ISBN.ToLower().Contains(query))
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetAvailableBooks()
        {
            return await _context.Books
                .Include(b => b.Supplier)
                .Where(b => b.AvailableCopies > 0)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthor(string author)
        {
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("El nombre del autor no puede estar vacío");

            return await _context.Books
                .Include(b => b.Supplier)
                .Where(b => b.Author.ToLower().Contains(author.ToLower()))
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByPublicationYear(int year)
        {
            if (year < 1000 || year > DateTime.Now.Year)
                throw new ArgumentException($"El año debe estar entre 1000 y {DateTime.Now.Year}");

            return await _context.Books
                .Include(b => b.Supplier)
                .Where(b => b.PublishedAt.HasValue && b.PublishedAt.Value.Year == year)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<int> GetTotalBookCount()
        {
            return await _context.Books.CountAsync();
        }

        public async Task<int> GetAvailableCopiesCount(int bookId)
        {
            var book = await _context.Books
                .Where(b => b.Id == bookId)
                .Select(b => b.AvailableCopies)
                .FirstOrDefaultAsync();

            return book;
        }
    }
}