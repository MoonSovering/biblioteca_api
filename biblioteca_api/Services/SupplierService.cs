using biblioteca_api.Data;
using biblioteca_api.Interfaces;
using biblioteca_api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace biblioteca_api.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly LibraryContext _context;

        public SupplierService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<Book> AddBook(int supplierId, Book book)
        {
            var supplier = await _context.Suppliers.FindAsync(supplierId);
            if (supplier == null)
                throw new KeyNotFoundException("Proveedor no encontrado");

            if (await _context.Books.AnyAsync(b => b.ISBN == book.ISBN))
                throw new InvalidOperationException("Ya existe un libro con este ISBN");

            book.SupplierId = supplierId;
            book.AvailableCopies = book.TotalCopies;

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return book;
        }

        public async Task UpdateBook(int supplierId, Book book)
        {
            if (!await IsBookFromSupplier(supplierId, book.Id))
                throw new UnauthorizedAccessException("El libro no pertenece a este proveedor");

            var existingBook = await _context.Books.FindAsync(book.Id);
            if (existingBook == null)
                throw new KeyNotFoundException("Libro no encontrado");

            var borrowedCopies = existingBook.TotalCopies - existingBook.AvailableCopies;
            if (book.TotalCopies < borrowedCopies)
                throw new InvalidOperationException($"No se pueden reducir las copias a menos de {borrowedCopies} (copias prestadas)");

            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.PublishedAt = book.PublishedAt;
            existingBook.Price = book.Price;
            existingBook.ISBN = book.ISBN;

            existingBook.AvailableCopies += (book.TotalCopies - existingBook.TotalCopies);
            existingBook.TotalCopies = book.TotalCopies;

            await _context.SaveChangesAsync();
        }

        public async Task RemoveBook(int supplierId, int bookId)
        {
            if (!await IsBookFromSupplier(supplierId, bookId))
                throw new UnauthorizedAccessException("El libro no pertenece a este proveedor");

            var book = await _context.Books
                .Include(b => b.Loans)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
                throw new KeyNotFoundException("Libro no encontrado");

            if (book.Loans.Any(l => l.Status == "Active"))
                throw new InvalidOperationException("No se puede eliminar un libro con préstamos activos");

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Book>> GetSuppliedBooks(int supplierId)
        {
            return await _context.Books
                .Where(b => b.SupplierId == supplierId)
                .OrderByDescending(b => b.TotalCopies)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetAvailableSuppliedBooks(int supplierId)
        {
            return await _context.Books
                .Where(b => b.SupplierId == supplierId && b.AvailableCopies > 0)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetBookLoans(int supplierId, int bookId)
        {
            if (!await IsBookFromSupplier(supplierId, bookId))
                throw new UnauthorizedAccessException("El libro no pertenece a este proveedor");

            return await _context.Loans
                .Include(l => l.User)
                .Where(l => l.BookId == bookId)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetAllSuppliedLoans(int supplierId)
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.Book.SupplierId == supplierId)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();
        }

        public async Task<int> GetSuppliedBooksCount(int supplierId)
        {
            return await _context.Books
                .Where(b => b.SupplierId == supplierId)
                .CountAsync();
        }

        public async Task<decimal> CalculateSupplierRevenue(int supplierId, DateTime startDate, DateTime endDate)
        {
            return await _context.Loans
                .Where(l => l.Book.SupplierId == supplierId &&
                           l.LoanDate >= startDate &&
                           l.LoanDate <= endDate)
                .SumAsync(l => l.Book.Price ?? 0);
        }

        public async Task<Dictionary<string, int>> GetLoanStatsBySupplier(int supplierId)
        {
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);

            var loans = await _context.Loans
                .Where(l => l.Book.SupplierId == supplierId && l.LoanDate >= sixMonthsAgo)
                .ToListAsync();

            return loans
                .GroupBy(l => new { l.LoanDate.Year, l.LoanDate.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .ToDictionary(
                    g => $"{g.Key.Year}-{g.Key.Month:D2}",
                    g => g.Count()
                );
        }

        public async Task<bool> IsBookFromSupplier(int supplierId, int bookId)
        {
            return await _context.Books
                .AnyAsync(b => b.Id == bookId && b.SupplierId == supplierId);
        }
    }
}