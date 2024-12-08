namespace Ecommerce.Exceptions
{
    public class UserAlreadyExists : AppException
    {
        public UserAlreadyExists(string message) : base (message, StatusCodes.Status400BadRequest) { }
    }
}
