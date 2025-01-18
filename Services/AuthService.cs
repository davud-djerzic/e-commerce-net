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
using Ecommerce.Services.ServiceInterfaces;
using System.Linq.Expressions;

namespace Ecommerce.Services
{
    public class AuthService(ApplicationDbContext _context, IConfiguration _configuration) : IAuthService
    {
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            List<User> users = await _context.Users.Where(u => !u.IsDeleted).ToListAsync();
            if (users.Count == 0) throw new NotFoundException("Users not found");

            return users;
        }

        public async Task<User> GetUserByIdAsync(int id) 
        {
            if (id <= 0) throw new BadRequestException("Id must be over a zero");

            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            if (user == null) throw new NotFoundException($"User {id} not found");

            return user;
        }

        public async Task DeleteUserAsync(int id)
        {
            if (id <= 0) throw new BadRequestException("Id must be over a zero");

            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            if (user == null) throw new NotFoundException($"User {id} not found");

            user.IsDeleted = true;
            user.Email = "Deleted_" + user.Email;
            user.Username = "Deleted_" + user.Username;

            List<Order> orders = await _context.Orders.Where(o => o.UserId == user.Id).ToListAsync();
            if (orders.Count > 0)
            {
                foreach (var order in orders)
                {
                    order.IsDeleted = true;
                    _context.Orders.Update(order);
                }
            }
            _context.Users.Update(user);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetSoftDeletedUsersAsync()
        {
            List<User> users = await _context.Users.Where(u => u.IsDeleted).ToListAsync();
            if (users.Count == 0) throw new NotFoundException("Users not found");

            return users;
        }

        public async Task RecoveryUserAsync(string username, string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password); // used to hash a password

            bool existingUser = await _context.Users.AnyAsync(u => u.Username == username);
            if (existingUser) throw new UserAlreadyExists("User already exist");

            User? user = await _context.Users.FirstOrDefaultAsync(u => (u.Username == "Deleted_" + username)); //&& (u.PasswordHash == passwordHash));
            
            if (user == null) throw new NotFoundException("User not found");

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) throw new BadRequestException("Incorrect username or password");

            user.Username = username;
            user.Email = user.Email.Substring(user.Email.IndexOf('_') + 1, user.Email.Length - user.Email.IndexOf('_') - 1);
            user.PasswordHash = passwordHash;
            user.IsDeleted = false;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task Register(UserRegisterDto userRegisterDto)
        {
            User user = new User(); // Create a new instance of user

            bool existingUser = await _context.Users // if user wants to use username or email which already exist
                .AnyAsync(u => u.Username == userRegisterDto.Username || u.Email == userRegisterDto.Email);

            if (existingUser) throw new UserAlreadyExists("Email or username already exist");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password); // used to hash a password

            if (userRegisterDto.Role != "Admin" && userRegisterDto.Role != "User") // if someone types wrong role, he will be User
                throw new BadRequestException("Bad input for role, must be admin or user!");
            else
            {
                user.Role = userRegisterDto.Role; // if user.Role == Admin or User
            }

            if (ValidateEmail(userRegisterDto.Email))
                user.Email = userRegisterDto.Email;

            else throw new BadRequestException("Incorrect type of email");

            user.IsDeleted = false;
            user.FirstName = userRegisterDto.FirstName;
            user.LastName = userRegisterDto.LastName;

            user.Username = userRegisterDto.Username;
            user.PasswordHash = passwordHash;
            userRegisterDto.Role = char.ToUpper(userRegisterDto.Role[0]) + userRegisterDto.Role.Substring(1).ToLower();
            // to make role with first letter upper and other lethers are lower

            _context.Users.Add(user); // add user to Users table
            await _context.SaveChangesAsync(); // save changes

        }

        public async Task<TokenResponseDto> Login(UserLoginDto userLoginDto)
        {
            User? userRequested = await _context.Users.FirstOrDefaultAsync(u => u.Username == userLoginDto.Username); // try to find user with typed username
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

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])); // made a key
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // made a creds for key
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1), // token expires after one day of signing
                signingCredentials: creds
            );

            return new TokenResponseDto(new JwtSecurityTokenHandler().WriteToken(token));
        }

        private bool ValidateEmail(string Email_Address)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            return regex.IsMatch(Email_Address);
        }
    }
}
