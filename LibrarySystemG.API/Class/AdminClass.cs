using LibrarySystemG.API.IRepository;
using LibrarySystemG.API.Model;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace LibrarySystemG.API.Class
{
    public class AdminClass : IAdminRepository
    {
        private readonly IConfiguration _config;

        // Constants for admin credentials (easy to change later)
        private const string AdminUsername = "geoadmin";
        private const string AdminPassword = "geo1234";
        private const string AdminRole = "Admin";

        public AdminClass(IConfiguration config)
        {
            _config = config;
        }

        public async Task<object> AdminLogin(AdminModel admin)
        {
            // simulate async operation
            await Task.Delay(50);

            // validate input
            if (admin == null ||
                string.IsNullOrWhiteSpace(admin.Username) ||
                string.IsNullOrWhiteSpace(admin.Password))
            {
                return new
                {
                    Success = false,
                    Message = "Username and password are required"
                };
            }

            // check credentials
            if (admin.Username == AdminUsername && admin.Password == AdminPassword)
            {
                var tokenService = new TokenService(_config);
                var token = tokenService.GenerateToken(admin.Username, AdminRole);

                return new
                {
                    Success = true,
                    Message = "Admin login successful",
                    Token = token
                };
            }

            // invalid login
            return new
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }
    }
}