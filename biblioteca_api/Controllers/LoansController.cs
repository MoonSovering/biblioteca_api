using biblioteca_api.Interfaces;
using biblioteca_api.Models.DTOs;
using biblioteca_api.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace biblioteca_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly IUserService _userService;

        public LoansController(ILoanService loanService, IUserService userService)
        {
            _loanService = loanService;
            _userService = userService;
        }

        [HttpPost("borrow")]
        public async Task<ActionResult<LoanDTO>> BorrowBook([FromBody] CreateLoanDTO loanDto)
        {
            try
            {
                var currentUserId = int.Parse(User.Identity.Name);
                var requestedUserId = loanDto.UserId;

                // Solo permitir que los asistentes/administradores creen préstamos para otros
                if (requestedUserId != currentUserId &&
                   !User.HasClaim("UserType", "Assistant") &&
                   !User.HasClaim("UserType", "Admin"))
                {
                    return Forbid();
                }

                var loan = await _loanService.BorrowBook(loanDto.BookId, requestedUserId);
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
                await _loanService.RenewLoan(renewDto.LoanId);
                var loan = await _loanService.GetLoanById(renewDto.LoanId);
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
            var currentUserId = int.Parse(User.Identity.Name);

            // Solo permitir ver préstamos propios o si es asistente/admin
            if (userId != currentUserId &&
               !User.HasClaim("UserType", "Assistant") &&
               !User.HasClaim("UserType", "Admin"))
            {
                return Forbid();
            }

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

        
        [HttpGet("range")]
        public async Task<ActionResult<IEnumerable<LoanDTO>>> GetLoansByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest(new { message = "La fecha de inicio no puede ser mayor que la fecha de fin" });

            var loans = await _loanService.GetLoansByDateRange(startDate, endDate);
            return Ok(loans);
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
            var currentUserId = int.Parse(User.Identity.Name);

            if (userId != currentUserId &&
               !User.HasClaim("UserType", "Assistant") &&
               !User.HasClaim("UserType", "Admin"))
            {
                return Forbid();
            }

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