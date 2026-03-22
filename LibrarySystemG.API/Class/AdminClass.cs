using LibrarySystemG.API.IRepository;
using LibrarySystemG.API.Model;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace LibrarySystemG.API.Class
{
    public class AdminClass : IAdminRepository
    {
        private readonly IConfiguration _config;

        public AdminClass(IConfiguration config)
        {
            _config = config;
        }

        public async Task<object> AdminLogin(AdminModel admin)
        {
            await Task.Delay(50);

            if (admin.Username == "geoadmin" && admin.Password == "geo1234")
            {
                var tokenService = new TokenService(_config);
                var token = tokenService.GenerateToken(admin.Username, "Admin");

                return new
                {
                    Success = true,
                    Message = "Admin login successful",
                    Token = token
                };
            }

            return new
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }
    }
}