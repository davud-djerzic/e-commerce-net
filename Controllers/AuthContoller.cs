using Ecommerce.Context;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Ecommerce.Controllers
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

        [HttpGet, Authorize(Roles = "Admin")] 
        public async Task <ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync(); // get all available users

            if (!users.Any()) return NotFound("Users not found"); // if there is no one

            return Ok(users); // return founded users
        }

        [HttpPost("register")]
        public async Task <ActionResult<User>> Register(UserRegisterDTO userRegisterDTO)
        {
            var user = new User(); // Create a new instance of user

            var existingUser = await _context.Users // if user wants to use username or email which already exist
                .AnyAsync(u => u.Username == userRegisterDTO.Username || u.Email == userRegisterDTO.Email); 

            if (existingUser) return BadRequest("Username or email already in use.");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userRegisterDTO.Password); // used to hash a password

            user.FirstName = userRegisterDTO.FirstName; 
            user.LastName = userRegisterDTO.LastName;
            if (ValidateEmail(userRegisterDTO.Email))
                user.Email = userRegisterDTO.Email;
            else return BadRequest("Incorrect format of email");

            user.Username = userRegisterDTO.Username;
            user.PasswordHash = passwordHash;
            userRegisterDTO.Role = char.ToUpper(userRegisterDTO.Role[0]) + userRegisterDTO.Role.Substring(1).ToLower(); 
            // to make role with first letter upper and other lethers are lower
             

            if (userRegisterDTO.Role != "Admin" && userRegisterDTO.Role != "User") // if someone types wrong role, he will be User
            {
                user.Role = "User";
            } else
            {
                user.Role = userRegisterDTO.Role; // if user.Role == Admin or User
            }
            _context.Users.Add(user); // add user to Users table
            await _context.SaveChangesAsync(); // save changes

            return NoContent(); 
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserLoginDTO userLoginDTO)
        {
            var userRequested = await _context.Users.FirstOrDefaultAsync(u => u.Username == userLoginDTO.Username); // try to find user with typed username
            if (userRequested == null) return BadRequest("User not found");

            //  if user type wrong password
            if (!BCrypt.Net.BCrypt.Verify(userLoginDTO.Password, userRequested.PasswordHash)) return BadRequest("User not found");

            //string token = CreateToken(userRequested);
            var claims = new[]
               {
                    new Claim(ClaimTypes.Role, userRequested.Role), // add role claim to jwt token data
                    //new Claim(JwtRegisteredClaimNames.Sub, userLoginDTO.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userRequested.Id.ToString()) // add ID claim to jwt token data
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])); // made a key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // made a creds for key
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1), // token expires after one day of signing
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var dbUsers = await _context.Users.FindAsync(id); // try to find a user with id
            if (dbUsers == null) return NotFound("User not found");

            _context.Users.Remove(dbUsers); // delete user
            await _context.SaveChangesAsync(); // save changes

            return Ok("User deleted");
        }

        public static bool ValidateEmail(string Email_Address)
        {
            //Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            return regex.IsMatch(Email_Address);
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
