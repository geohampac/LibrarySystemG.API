using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using LibrarySystemG.API.IRepository;
using LibrarySystemG.API.Model;
using LibrarySystemG.API.Model.Request;
using LibrarySystemG.API.Model.Response;
using Microsoft.Extensions.Configuration;

namespace LibrarySystemG.API.Class
{
    public class LoginClass : ILoginRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _config;

        public LoginClass(IConfiguration config)
        {
            _config = config;
            _connectionString = config.GetConnectionString("trackerlibrary");
        }

        public async Task<ServiceResponse<LoginResponseModel>> Login(LoginModel model)
        {
            var service = new ServiceResponse<LoginResponseModel>();

            using (var conn = new SqlConnection(_connectionString))
            {
                var param = new DynamicParameters();
                param.Add("@Username", model.Username);
                param.Add("@Password", model.Password);

                var user = await conn.QueryFirstOrDefaultAsync<LoginResponseModel>(
                    "LIBRARYSYSTEM_LOGINUSER",
                    param,
                    commandType: CommandType.StoredProcedure
                );

                if (user == null)
                {
                    service.Status = 400;
                    service.Success = false;
                    service.Message = "Invalid username or password";
                    return service;
                }

                
                var tokenService = new TokenService(_config);
                string token = tokenService.GenerateToken(user.Username, "User");

                user.Password = null; 
                user.Token = token;

                service.Status = 200;
                service.Success = true;
                service.Message = "Login successful";
                service.Data = user;
            }

            return service;
        }
    }
}