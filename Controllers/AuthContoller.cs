using Ecommerce.Models;
using Ecommerce.Models.RequestDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Exceptions;
using Ecommerce.Services.ServiceInterfaces;
using Microsoft.AspNetCore.Cors;
using Ecommerce.Models.ResponseDto;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors("AllowAll")]
    public class AuthContoller(IAuthService _authService) : ControllerBase
    {
        [HttpGet, Authorize(Roles = "Admin")] 
        public async Task <ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                IEnumerable<User> users = await _authService.GetUsersAsync();
                return Ok(users);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpGet("getById"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            try
            {
                User user = await _authService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpGet("getSoftDeleted"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetSoftDeletedUsers()
        {
            try
            {
                IEnumerable<User> users = await _authService.GetSoftDeletedUsersAsync();
                return Ok(users);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpPut("Recovery/{username}, {password}")]
        public async Task<ActionResult<User>> RecoveryUser(string username, string password)
        {
            try
            {
                await _authService.RecoveryUserAsync(username, password);
                return Ok("Your account is recovered");
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (UserAlreadyExists ex)
            {
                return BadRequest(new { ex = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
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
            catch (UserAlreadyExists ex)
            {
                return BadRequest(new {ex = ex.Message});
            }
            catch(BadRequestException ex)
            {
                return BadRequest(new {ex = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            try
            {
                TokenResponseDto tokenResponse = await _authService.Login(userLoginDto);
                return Ok(tokenResponse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> DeleteUserAsync(int id)
        {
            try
            {
                await _authService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return BadRequest(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }


        /*
        private string CreateToken(User user)
        {
            List <Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), 
                new Claim("FirstName", user.FirstName), // Ime
                new Claim("LastName", user.LastName), // Prezime
                new Claim(JwtRegisteredClaimNames.Email, user.Email), // Email
                new Claim("Username", user.Username), // Korisničko ime
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
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
