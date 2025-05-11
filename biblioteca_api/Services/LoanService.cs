using biblioteca_api.Data;
using biblioteca_api.Helpers.Mappers;
using biblioteca_api.Interfaces;
using biblioteca_api.Models.DTOs;
using biblioteca_api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace biblioteca_api.Services
{
    public class LoanService : ILoanService
    {
        private readonly LibraryContext _context;
        private readonly IUserService _userService;

        public LoanService(LibraryContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<LoanDTO> BorrowBook(int bookId, int userId)
        {
            var book = await _context.Books.FindAsync(bookId)
                ?? throw new KeyNotFoundException("Libro no encontrado");

            var user = await _userService.GetById(userId);

            if (!user.IsReader())
                throw new InvalidOperationException("Solo los lectores pueden tomar prestados libros");

            if (!book.IsAvailable())
                throw new InvalidOperationException("No hay copias disponibles de este libro");

            if (await _context.Loans.AnyAsync(l => l.BookId == bookId && l.UserId == userId && l.Status == "Active"))
                throw new InvalidOperationException("Ya tienes este libro prestado");

            if (user.HasOverdueLoans())
                throw new InvalidOperationException("Tienes préstamos vencidos. Regulariza tu situación primero");

            if (!user.CanBorrowBooks())
                throw new InvalidOperationException("Has alcanzado el límite de préstamos");

            var loan = new Loan
            {
                BookId = bookId,
                UserId = userId,
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14),
                Status = "Active"
            };

            book.BorrowCopy();
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return await GetLoanById(loan.Id);
        }

        public async Task ReturnBook(int loanId)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == loanId)
                ?? throw new KeyNotFoundException("Préstamo no encontrado");

            if (loan.Status == "Returned")
                throw new InvalidOperationException("Este libro ya fue devuelto");

            loan.MarkAsReturned();
            await _context.SaveChangesAsync();
        }

        public async Task<LoanDTO> RenewLoan(int loanId, int additionalDays)
        {
            var loan = await _context.Loans.FindAsync(loanId)
                ?? throw new KeyNotFoundException("Préstamo no encontrado");

            if (loan.Status != "Active")
                throw new InvalidOperationException("Solo se pueden renovar préstamos activos");

            if (loan.IsOverdue())
                throw new InvalidOperationException("No se pueden renovar préstamos vencidos");

            loan.DueDate = loan.DueDate.AddDays(additionalDays);
            await _context.SaveChangesAsync();

            return await GetLoanById(loanId);
        }

        public async Task<LoanDTO> GetLoanById(int loanId)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == loanId)
                ?? throw new KeyNotFoundException("Préstamo no encontrado");

            return LoanMapper.MapToDTO(loan);
        }

        public async Task<IEnumerable<LoanDTO>> GetUserLoans(int userId)
        {
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();

            return LoanMapper.MapToDTO(loans);
        }

        public async Task<IEnumerable<LoanDTO>> GetActiveLoans()
        {
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.Status == "Active")
                .OrderBy(l => l.DueDate)
                .ToListAsync();

            return LoanMapper.MapToDTO(loans);
        }

        public async Task<IEnumerable<LoanDTO>> GetOverdueLoans()
        {
            var now = DateTime.UtcNow;
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.Status == "Active" && l.DueDate < now)
                .OrderBy(l => l.DueDate)
                .ToListAsync();

            return LoanMapper.MapToDTO(loans);
        }

        public async Task<bool> CanUserBorrow(int userId)
        {
            var user = await _userService.GetById(userId);
            return user.CanBorrowBooks();
        }

        public async Task<bool> IsBookAvailable(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            return book?.IsAvailable() ?? false;
        }

        public async Task<int> GetActiveLoansCount()
        {
            return await _context.Loans
                .Where(l => l.Status == "Active")
                .CountAsync();
        }

        public async Task<int> GetUserActiveLoansCount(int userId)
        {
            return await _context.Loans
                .Where(l => l.UserId == userId && l.Status == "Active")
                .CountAsync();
        }
    }
}