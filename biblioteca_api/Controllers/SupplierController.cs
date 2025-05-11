using biblioteca_api.Interfaces;
using biblioteca_api.Models.DTOs;
using biblioteca_api.Models.DTOs.Book;
using biblioteca_api.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace biblioteca_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly IUserService _userService;

        public SuppliersController(ISupplierService supplierService, IUserService userService)
        {
            _supplierService = supplierService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierDTO>>> GetAllSuppliers()
        {
            var suppliers = await _userService.GetAll();
            return Ok(suppliers);
        }

        [HttpGet("{supplierId}")]
        public async Task<ActionResult<SupplierDTO>> GetSupplierById(int supplierId)
        {
            if (!await HasAccessToSupplier(supplierId))
                return Forbid();

            try
            {
                var supplier = await _userService.GetById(supplierId);
                return Ok(supplier);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        
        [HttpPost("register")]
        public async Task<ActionResult<SupplierDTO>> RegisterSupplier([FromBody] CreateSupplierDTO supplierDto)
        {
            try
            {
                var supplier = new Supplier
                {
                    CompanyName = supplierDto.CompanyName,
                    TaxId = supplierDto.TaxId,
                    Email = supplierDto.Email,
                    UserType = "Supplier",
                    Username = supplierDto.Email
                };

                var createdSupplier = await _userService.Create(supplier, supplierDto.Password);
                return CreatedAtAction(nameof(GetSupplierById), new { supplierId = createdSupplier.Id }, createdSupplier);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpPut("{supplierId}")]
        public async Task<IActionResult> UpdateSupplier(int supplierId, [FromBody] UpdateSupplierDTO supplierDto)
        {
            if (!await HasAccessToSupplier(supplierId) || supplierId != supplierDto.Id)
                return Forbid();

            try
            {
                var supplier = new Supplier
                {
                    Id = supplierDto.Id,
                    CompanyName = supplierDto.CompanyName,
                    TaxId = supplierDto.TaxId,
                    Email = supplierDto.Email,
                    IsActive = supplierDto.IsActive
                };

                await _userService.Update(supplier, null);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpGet("{supplierId}/books")]
        public async Task<ActionResult<IEnumerable<Book>>> GetSuppliedBooks(int supplierId)
        {
            if (!await HasAccessToSupplier(supplierId))
                return Forbid();

            try
            {
                var books = await _supplierService.GetSuppliedBooks(supplierId);
                return Ok(books);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        
        [HttpGet("{supplierId}/books/available")]
        public async Task<ActionResult<IEnumerable<Book>>> GetAvailableSuppliedBooks(int supplierId)
        {
            if (!await HasAccessToSupplier(supplierId))
                return Forbid();

            try
            {
                var books = await _supplierService.GetAvailableSuppliedBooks(supplierId);
                return Ok(books);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        
        [HttpPost("{supplierId}/books")]
        public async Task<ActionResult<Book>> AddBook(int supplierId, [FromBody] CreateBookDTO bookDto)
        {
            if (!await HasAccessToSupplier(supplierId))
                return Forbid();

            try
            {
                var book = new Book
                {
                    Title = bookDto.Title,
                    Author = bookDto.Author,
                    PublishedAt = bookDto.PublishedAt,
                    Price = bookDto.Price,
                    ISBN = bookDto.ISBN,
                    TotalCopies = bookDto.TotalCopies,
                    SupplierId = supplierId
                };

                var createdBook = await _supplierService.AddBook(supplierId, book);
                return CreatedAtAction(nameof(GetSuppliedBooks), new { supplierId }, createdBook);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpPut("{supplierId}/books/{bookId}")]
        public async Task<IActionResult> UpdateBook(int supplierId, int bookId, [FromBody] UpdateBookDTO bookDto)
        {
            if (!await HasAccessToSupplier(supplierId) || bookId != bookDto.Id)
                return Forbid();

            try
            {
                var book = new Book
                {
                    Id = bookDto.Id,
                    Title = bookDto.Title,
                    Author = bookDto.Author,
                    PublishedAt = bookDto.PublishedAt,
                    Price = bookDto.Price,
                    ISBN = bookDto.ISBN,
                    TotalCopies = bookDto.TotalCopies
                };

                await _supplierService.UpdateBook(supplierId, book);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpDelete("{supplierId}/books/{bookId}")]
        public async Task<IActionResult> RemoveBook(int supplierId, int bookId)
        {
            if (!await HasAccessToSupplier(supplierId))
                return Forbid();

            try
            {
                await _supplierService.RemoveBook(supplierId, bookId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpGet("{supplierId}/books/{bookId}/loans")]
        public async Task<ActionResult<IEnumerable<Loan>>> GetBookLoans(int supplierId, int bookId)
        {
            if (!await HasAccessToSupplier(supplierId))
                return Forbid();

            try
            {
                var loans = await _supplierService.GetBookLoans(supplierId, bookId);
                return Ok(loans);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }

        
        [HttpGet("{supplierId}/loans")]
        public async Task<ActionResult<IEnumerable<Loan>>> GetAllSuppliedLoans(int supplierId)
        {
            if (!await HasAccessToSupplier(supplierId))
                return Forbid();

            try
            {
                var loans = await _supplierService.GetAllSuppliedLoans(supplierId);
                return Ok(loans);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        
        [HttpGet("{supplierId}/stats")]
        public async Task<ActionResult<SupplierStatsDTO>> GetSupplierStats(int supplierId)
        {
            if (!await HasAccessToSupplier(supplierId))
                return Forbid();

            try
            {
                var stats = new SupplierStatsDTO
                {
                    TotalBooksSupplied = await _supplierService.GetSuppliedBooksCount(supplierId),
                    ActiveLoans = (await _supplierService.GetAllSuppliedLoans(supplierId)).Count(),
                    TotalRevenue = await _supplierService.CalculateSupplierRevenue(supplierId, DateTime.UtcNow.AddYears(-1), DateTime.UtcNow),
                    LoansLastSixMonths = await _supplierService.GetLoanStatsBySupplier(supplierId)
                };
                return Ok(stats);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

       
        private async Task<bool> HasAccessToSupplier(int supplierId)
        {
            var currentUserId = int.Parse(User.Identity.Name);
            var currentUserType = User.FindFirst("UserType")?.Value;

            if (currentUserType == "Admin")
                return true;

            return currentUserId == supplierId && currentUserType == "Supplier";
        }
    }
}