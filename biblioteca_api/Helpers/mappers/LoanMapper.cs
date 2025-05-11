using biblioteca_api.Models.DTOs;
using biblioteca_api.Models.Entities;

namespace biblioteca_api.Helpers.Mappers
{
    public static class LoanMapper
    {
        public static LoanDTO MapToDTO(Loan loan)
        {
            if (loan == null) return null;

            return new LoanDTO
            {
                Id = loan.Id,
                BookId = loan.BookId,
                BookTitle = loan.Book?.Title,
                UserId = loan.UserId,
                UserName = loan.User?.Username,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                Status = loan.Status,
                IsOverdue = loan.IsOverdue()
            };
        }

        public static IEnumerable<LoanDTO> MapToDTO(IEnumerable<Loan> loans)
        {
            return loans.Select(loan => MapToDTO(loan));
        }
    }
}