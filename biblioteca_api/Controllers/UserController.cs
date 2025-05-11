using biblioteca_api.Interfaces;
using biblioteca_api.Models.DTOs;
using biblioteca_api.Models.Entities;
using biblioteca_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace biblioteca_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthDTO model)
        {
            try
            {
                var user = await _userService.Authenticate(model.Username, model.Password);
                return Ok(user);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var currentUserId = int.Parse(User.Identity.Name);
            var currentUser = await _userService.GetById(currentUserId);

            // Solo permitir acceso si es el mismo usuario o un admin
            if (id != currentUserId && !currentUser.IsAdmin())
                return Forbid();

            try
            {
                var user = await _userService.GetById(id);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO request)
        {
            try
            {
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    UserType = request.UserType ?? "Reader" // Valor por defecto
                };

                var createdUser = await _userService.Create(user, request.Password);
                return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDTO request)
        {
            var currentUserId = int.Parse(User.Identity.Name);
            var currentUser = await _userService.GetById(currentUserId);

            // Solo permitir actualización si es el mismo usuario o un admin
            if (id != currentUserId && !currentUser.IsAdmin())
                return Forbid();

            try
            {
                var user = new User
                {
                    Id = id,
                    Username = request.Username,
                    Email = request.Email,
                    UserType = request.UserType,
                    IsActive = request.IsActive
                };

                await _userService.Update(user, request.Password);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.Delete(id);
            return NoContent();
        }
    }
}