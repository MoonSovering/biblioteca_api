using biblioteca_api.Interfaces;
using biblioteca_api.Models.DTOs;
using biblioteca_api.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace biblioteca_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetAll()
        {
            var books = await _bookService.GetAll();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDTO>> GetById(int id)
        {
            try
            {
                var book = await _bookService.GetById(id);
                return Ok(book);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireAdminOrSupplier")]
        [HttpPost]
        public async Task<ActionResult<BookDTO>> Create([FromBody] CreateBookDTO bookDto)
        {
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
                    AvailableCopies = bookDto.TotalCopies,
                    SupplierId = bookDto.SupplierId
                };

                var createdBook = await _bookService.Create(book);
                return CreatedAtAction(nameof(GetById), new { id = createdBook.Id }, createdBook);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireAdminOrSupplier")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDTO bookDto)
        {
            if (id != bookDto.Id)
                return BadRequest(new { message = "ID del libro no coincide" });

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

                await _bookService.Update(book);
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

        [Authorize(Policy = "RequireAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _bookService.Delete(id);
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

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookSearchResultDTO>>> Search([FromQuery] string query)
        {
            var books = await _bookService.Search(query);
            return Ok(books);
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetAvailableBooks()
        {
            var books = await _bookService.GetAvailableBooks();
            return Ok(books);
        }

        [HttpGet("author/{author}")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetByAuthor(string author)
        {
            var books = await _bookService.GetBooksByAuthor(author);
            return Ok(books);
        }

        [HttpGet("year/{year}")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetByPublicationYear(int year)
        {
            var books = await _bookService.GetBooksByPublicationYear(year);
            return Ok(books);
        }
    }
}