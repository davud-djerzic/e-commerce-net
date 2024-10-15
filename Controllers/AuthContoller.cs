using E_commerce_API.Context;
using E_commerce_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthContoller : ControllerBase
    {
        //public static User user = new User();

        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthContoller(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet] 
        public async Task <ActionResult<IEnumerable<User>>> getUsers()
        {
            var users = await _context.Users.ToListAsync();

            return Ok(users);
        }

        [HttpPost("register")]
        public async Task <ActionResult<User>> Register(UserRegisterDTO userRegisterDTO)
        {
            var user = new User(); // Kreiraj novu instancu korisnika

            var existingUser = await _context.Users
                .AnyAsync(u => u.Username == user.Username || u.Email == user.Email);

            if (existingUser)
            {
                return BadRequest("Username or email already in use.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userRegisterDTO.Password);

            user.FirstName = userRegisterDTO.FirstName;
            user.LastName = userRegisterDTO.LastName;
            user.Email = userRegisterDTO.Email;
            user.Username = userRegisterDTO.Username;
            user.PasswordHash = passwordHash;
            userRegisterDTO.Role = char.ToUpper(userRegisterDTO.Role[0]) + userRegisterDTO.Role.Substring(1).ToLower();
            if (userRegisterDTO.Role != "Admin" && userRegisterDTO.Role != "User")
            {
                user.Role = "User";
            } else
            {
                user.Role = userRegisterDTO.Role;
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserLoginDTO userLoginDTO)
        {
            var userRequested = await _context.Users.FirstOrDefaultAsync(u => u.Username == userLoginDTO.Username);
            if (userRequested == null)
            {
                return BadRequest("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(userLoginDTO.Password, userRequested.PasswordHash)) {
                return BadRequest("User not found");
            }

            //string token = CreateToken(userRequested);
            var claims = new[]
               {
                    new Claim(ClaimTypes.Role, userRequested.Role),
                    new Claim(JwtRegisteredClaimNames.Sub, userLoginDTO.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });

        }

        [HttpDelete]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var dbUsers = await _context.Users.FindAsync(id);
            if (dbUsers == null)
                return NotFound("Product not found");

            _context.Users.Remove(dbUsers);
            await _context.SaveChangesAsync();

            return Ok("Product deleted");
        }



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
        }
    }
}
