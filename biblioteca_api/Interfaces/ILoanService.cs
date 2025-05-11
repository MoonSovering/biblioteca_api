using biblioteca_api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace biblioteca_api.Interfaces
{
    public interface ILoanService
    {
        Task<LoanDTO> BorrowBook(int bookId, int userId);
        Task ReturnBook(int loanId);
        Task<LoanDTO> RenewLoan(int loanId, int additionalDays);

        Task<LoanDTO> GetLoanById(int loanId);
        Task<IEnumerable<LoanDTO>> GetUserLoans(int userId);
        Task<IEnumerable<LoanDTO>> GetActiveLoans();
        Task<IEnumerable<LoanDTO>> GetOverdueLoans();
        Task<bool> CanUserBorrow(int userId);
        Task<bool> IsBookAvailable(int bookId);

        Task<int> GetActiveLoansCount();
        Task<int> GetUserActiveLoansCount(int userId);
    }
}