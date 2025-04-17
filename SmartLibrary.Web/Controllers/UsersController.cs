using Microsoft.AspNetCore.Mvc;
using SmartLibrary.Application.Services;

namespace SmartLibrary.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
        {
            try
            {
                await _userService.RegisterUserAsync(
                    firstname: request.FirstName,
                    lastname: request.LastName,
                    email: request.Email,
                    password: request.Password,
                    membershipTypeId: request.MembershipTypeId
                );

                return Ok(new { Message = "User registered successfully!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }

    public class RegisterUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int MembershipTypeId { get; set; } = 1; // Default value
    }
}