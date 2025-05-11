using biblioteca_api.Interfaces;
using biblioteca_api.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace biblioteca_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpPost("borrow")]
        public async Task<ActionResult<LoanDTO>> BorrowBook([FromBody] CreateLoanDTO loanDto)
        {
            try
            {
                var loan = await _loanService.BorrowBook(loanDto.BookId, loanDto.UserId);
                return Ok(loan);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("return")]
        public async Task<IActionResult> ReturnBook([FromBody] ReturnLoanDTO returnDto)
        {
            try
            {
                await _loanService.ReturnBook(returnDto.LoanId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("renew")]
        public async Task<ActionResult<LoanDTO>> RenewLoan([FromBody] RenewLoanDTO renewDto)
        {
            try
            {
                var loan = await _loanService.RenewLoan(renewDto.LoanId, renewDto.AdditionalDays);
                return Ok(loan);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<LoanDTO>>> GetUserLoans(int userId)
        {
            try
            {
                var loans = await _loanService.GetUserLoans(userId);
                return Ok(loans);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<LoanDTO>>> GetActiveLoans()
        {
            var loans = await _loanService.GetActiveLoans();
            return Ok(loans);
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<LoanDTO>>> GetOverdueLoans()
        {
            var loans = await _loanService.GetOverdueLoans();
            return Ok(loans);
        }

        [HttpGet("stats")]
        public async Task<ActionResult<LoanStatsDTO>> GetLoanStats()
        {
            var stats = new LoanStatsDTO
            {
                TotalActiveLoans = await _loanService.GetActiveLoansCount(),
                TotalOverdueLoans = (await _loanService.GetOverdueLoans()).Count()
            };
            return Ok(stats);
        }

        [HttpGet("check-availability/{bookId}")]
        public async Task<ActionResult<bool>> CheckBookAvailability(int bookId)
        {
            try
            {
                var isAvailable = await _loanService.IsBookAvailable(bookId);
                return Ok(isAvailable);
            }
            catch (Exception)
            {
                return Ok(false);
            }
        }

        [HttpGet("can-borrow/{userId}")]
        public async Task<ActionResult<bool>> CanUserBorrow(int userId)
        {
            try
            {
                var canBorrow = await _loanService.CanUserBorrow(userId);
                return Ok(canBorrow);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}