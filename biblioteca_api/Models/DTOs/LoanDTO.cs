using System.ComponentModel.DataAnnotations;

namespace biblioteca_api.Models.DTOs
{
    public class LoanDTO
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; }
        public bool IsOverdue { get; set; }
    }

    public class CreateLoanDTO
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        public int UserId { get; set; }
    }

    public class ReturnLoanDTO
    {
        [Required]
        public int LoanId { get; set; }
    }

    public class RenewLoanDTO
    {
        [Required]
        public int LoanId { get; set; }
        public int AdditionalDays { get; set; } = 7;
    }

    public class LoanStatsDTO
    {
        public int TotalActiveLoans { get; set; }
        public int TotalOverdueLoans { get; set; }
    }
}