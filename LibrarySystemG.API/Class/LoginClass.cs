using System.Data;
using System.Data.SqlClient;
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
        private readonly TokenService _tokenService;

        public LoginClass(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("trackerlibrary");
            _tokenService = new TokenService(config);
        }

        public async Task<ServiceResponse<LoginResponseModel>> Login(LoginModel model)
        {
            var response = new ServiceResponse<LoginResponseModel>();

            try
            {
                using var conn = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@Username", model.Username);
                parameters.Add("@Password", model.Password);

                var user = await conn.QueryFirstOrDefaultAsync<LoginResponseModel>(
                    "LIBRARYSYSTEM_LOGINUSER",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (user == null)
                {
                    return new ServiceResponse<LoginResponseModel>
                    {
                        Status = 400,
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                // Generate Token
                user.Token = _tokenService.GenerateToken(user.Username, "User");

                // Hide password
                user.Password = null;

                return new ServiceResponse<LoginResponseModel>
                {
                    Status = 200,
                    Success = true,
                    Message = "Login successful",
                    Data = user
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoginResponseModel>
                {
                    Status = 500,
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }
    }
}