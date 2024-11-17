using Ecommerce.Context;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Exceptions;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ecommerce.Models.ResponseDto;
using Ecommerce.Models.RequestDto;

namespace Ecommerce.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            if (!users.Any()) throw new NotFoundException("Users not found");

            return users;
        }

        public async Task<User> GetUserByIdAsync(int id) 
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new NotFoundException("User not found");

            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;
            
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task Register(UserRegisterDto userRegisterDto)
        {
            var user = new User(); // Create a new instance of user

            var existingUser = await _context.Users // if user wants to use username or email which already exist
                .AnyAsync(u => u.Username == userRegisterDto.Username || u.Email == userRegisterDto.Email);

            if (existingUser) throw new NotFoundException("Email or username already exist");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password); // used to hash a password

            user.FirstName = userRegisterDto.FirstName;
            user.LastName = userRegisterDto.LastName;

            if (ValidateEmail(userRegisterDto.Email))
                user.Email = userRegisterDto.Email;

            else throw new NotFoundException("Incorrect type of email");

            user.Username = userRegisterDto.Username;
            user.PasswordHash = passwordHash;
            userRegisterDto.Role = char.ToUpper(userRegisterDto.Role[0]) + userRegisterDto.Role.Substring(1).ToLower();
            // to make role with first letter upper and other lethers are lower

            if (userRegisterDto.Role != "Admin" && userRegisterDto.Role != "User") // if someone types wrong role, he will be User
            {
                user.Role = "User";
            }
            else
            {
                user.Role = userRegisterDto.Role; // if user.Role == Admin or User
            }
            _context.Users.Add(user); // add user to Users table
            await _context.SaveChangesAsync(); // save changes

        }

        public async Task<TokenResponseDto> Login(UserLoginDto userLoginDto)
        {
            var userRequested = await _context.Users.FirstOrDefaultAsync(u => u.Username == userLoginDto.Username); // try to find user with typed username
            if (userRequested == null) throw new NotFoundException("User not found");

            //  if user type wrong password
            if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, userRequested.PasswordHash)) throw new NotFoundException("User not found");

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

            return new TokenResponseDto(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public static bool ValidateEmail(string Email_Address)
        {
            //Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            return regex.IsMatch(Email_Address);
        }
    }
}
