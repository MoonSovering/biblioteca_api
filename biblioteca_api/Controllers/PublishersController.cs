using biblioteca_api.Interfaces;
using biblioteca_api.Models.DTOs.Publisher;
using Microsoft.AspNetCore.Mvc;

namespace biblioteca_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublisherDTO>>> GetAll()
        {
            var publishers = await _publisherService.GetAll();
            return Ok(publishers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PublisherDTO>> GetById(int id)
        {
            try
            {
                var publisher = await _publisherService.GetById(id);
                return Ok(publisher);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<PublisherDTO>> Create([FromBody] CreatePublisherDTO publisherDto)
        {
            try
            {
                var createdPublisher = await _publisherService.Create(publisherDto);
                return CreatedAtAction(nameof(GetById), new { id = createdPublisher.Id }, createdPublisher);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreatePublisherDTO publisherDto)
        {
            try
            {
                await _publisherService.Update(id, publisherDto);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _publisherService.Delete(id);
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
        public async Task<ActionResult<IEnumerable<PublisherDTO>>> Search([FromQuery] string query)
        {
            var publishers = await _publisherService.Search(query);
            return Ok(publishers);
        }
    }
}