using biblioteca_api.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace biblioteca_api.Interfaces
{
    public interface ILoanService
    {
        Task<Loan> BorrowBook(int bookId, int userId);
        Task ReturnBook(int loanId);
        Task RenewLoan(int loanId);

        Task<Loan> GetLoanById(int loanId);
        Task<IEnumerable<Loan>> GetUserLoans(int userId);
        Task<IEnumerable<Loan>> GetActiveLoans();
        Task<IEnumerable<Loan>> GetOverdueLoans();
        Task<IEnumerable<Loan>> GetLoansByDateRange(DateTime startDate, DateTime endDate);

        Task<bool> CanUserBorrow(int userId);
        Task<bool> IsBookAvailable(int bookId);

        Task<int> GetActiveLoansCount();
        Task<int> GetUserActiveLoansCount(int userId);
    }
}