using System.Data;
using System.Data.SqlClient;
using Dapper;
using LibrarySystemG.API.IRepository;
using LibrarySystemG.API.Model;
using LibrarySystemG.API.Model.Response;

namespace LibrarySystemG.API.Class
{
    public class RegisterClass : IRegisterRepository
    {
        private readonly string _connectionString;

        public RegisterClass(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("trackerlibrary");
        }

        public async Task<ServiceResponse<RegisterResponseModel>> Register(RegisterModel model)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@FirstName", model.FirstName);
                parameters.Add("@LastName", model.LastName);
                parameters.Add("@Username", model.Username);
                parameters.Add("@Password", model.Password);
                parameters.Add("@Course", model.Course);
                parameters.Add("@Email", model.Email);

                var result = await conn.QueryFirstOrDefaultAsync<RegisterResponseModel>(
                    "LIBRARYSYSTEM_REGISTER",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (result == null)
                {
                    return new ServiceResponse<RegisterResponseModel>
                    {
                        Status = 400,
                        Success = false,
                        Message = "Registration failed"
                    };
                }

                return new ServiceResponse<RegisterResponseModel>
                {
                    Status = 200,
                    Success = true,
                    Message = result.Message,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<RegisterResponseModel>
                {
                    Status = 500,
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }
    }
}x`