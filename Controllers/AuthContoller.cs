using Ecommerce.Models;
using Ecommerce.Models.RequestDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Services;
using Ecommerce.Exceptions;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthContoller : ControllerBase
    {
        //public static User user = new User();

        private readonly IAuthService _authService;
        public AuthContoller(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet, Authorize(Roles = "Admin")] 
        public async Task <ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var users = await _authService.GetUsersAsync();
                return Ok(users);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("getById"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> GetUserById([FromQuery] int id)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            try
            {
                await _authService.Register(userRegisterDto);
                return Created();
            }
            catch (NotFoundException ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            try
            {
                var tokenResponse = await _authService.Login(userLoginDto);
                return Ok(tokenResponse);
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> DeleteUserAsync(int id)
        {
            var user = await _authService.DeleteUserAsync(id);
            if (!user) return NotFound("User not found");

            return Ok("User deleted");
        }


        /*
        private string CreateToken(User user)
        {
            List <Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Id korisnika
                new Claim("FirstName", user.FirstName), // Ime
                new Claim("LastName", user.LastName), // Prezime
                new Claim(JwtRegisteredClaimNames.Email, user.Email), // Email
                new Claim("Username", user.Username), // Korisničko ime
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Jedinstveni identifikator za JWT
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Iss, "yourIssuer"),
                new Claim(JwtRegisteredClaimNames.Aud, "yourAudience")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1), // expires after 1 hour
                    signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }*/
    }
}
