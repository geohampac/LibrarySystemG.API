using System.Data;
using System.Data.SqlClient;
using Dapper;
using LibrarySystemG.API.IRepository;
using LibrarySystemG.API.Model;
using LibrarySystemG.API.Model.Request;
using LibrarySystemG.API.Model.Response;

namespace LibrarySystemG.API.Class
{
    public class LoginClass : ILoginRepository
    {
        private readonly string _connectionString;
        private readonly TokenService _tokenService;

        private const string LoginStoredProcedure = "LIBRARYSYSTEM_LOGINUSER";
        private const string DefaultRole = "User";

        public LoginClass(IConfiguration config, TokenService tokenService)
        {
            _connectionString = config.GetConnectionString("trackerlibrary");
            _tokenService = tokenService;
        }

        public async Task<ServiceResponse<LoginResponseModel>> Login(LoginModel model)
        {
            if (!IsValidRequest(model))
                return BadRequest("Username and password are required");

            try
            {
                using var conn = CreateConnection();

                var user = await conn.QueryFirstOrDefaultAsync<LoginResponseModel>(
                    LoginStoredProcedure,
                    new
                    {
                        model.Username,
                        model.Password
                    },
                    commandType: CommandType.StoredProcedure
                );

                if (user == null)
                    return Unauthorized("Invalid username or password");

                user.Token = _tokenService.GenerateToken(user.Username, DefaultRole);
                user.Password = null;

                return Success("Login successful", user);
            }
            catch
            {
                // Don't expose internal errors in production
                return Error("An unexpected error occurred");
            }
        }

        // 🔹 Helpers

        private IDbConnection CreateConnection() =>
            new SqlConnection(_connectionString);

        private static bool IsValidRequest(LoginModel model) =>
            model != null &&
            !string.IsNullOrWhiteSpace(model.Username) &&
            !string.IsNullOrWhiteSpace(model.Password);

        private static ServiceResponse<LoginResponseModel> Success(string message, LoginResponseModel data) =>
            new()
            {
                Status = 200,
                Success = true,
                Message = message,
                Data = data
            };

        private static ServiceResponse<LoginResponseModel> BadRequest(string message) =>
            new()
            {
                Status = 400,
                Success = false,
                Message = message
            };

        private static ServiceResponse<LoginResponseModel> Unauthorized(string message) =>
            new()
            {
                Status = 401,
                Success = false,
                Message = message
            };

        private static ServiceResponse<LoginResponseModel> Error(string message) =>
            new()
            {
                Status = 500,
                Success = false,
                Message = message
            };
    }
}