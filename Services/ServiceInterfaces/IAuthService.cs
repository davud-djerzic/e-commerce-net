using Ecommerce.Models;
using Ecommerce.Models.ResponseDto;
using Ecommerce.Models.RequestDto;

namespace Ecommerce.Services.ServiceInterfaces
{
    public interface IAuthService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task Register(UserRegisterDto userRegisterDto);
        Task<TokenResponseDto> Login(UserLoginDto userLoginDto);
        Task DeleteUserAsync(int id);
        Task<IEnumerable<User>> GetSoftDeletedUsersAsync();
        Task RecoveryUserAsync(string username, string password);
    }
}
