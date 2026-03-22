using System.Data;
using System.Data.SqlClient;
using Dapper;
using LibrarySystemG.API.IRepository;
using LibrarySystemG.API.Model.Request;
using LibrarySystemG.API.Model.Response;

namespace LibrarySystemG.API.Services
{
    public class LoginService : ILoginRepository
    {
        private readonly string _connectionString;

        public LoginService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("librarysystem"); // fix connection string
        }

        public async Task<ServiceResponse<LoginResponseModel>> Login(LoginModel model)
        {
            var service = new ServiceResponse<LoginResponseModel>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var param = new DynamicParameters();
                    param.Add("@Username", model.Username);
                    param.Add("@Password", model.Password); // ✅ send password

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
                        service.Data = null;
                    }
                    else
                    {
                        service.Status = 200;
                        service.Success = true;
                        service.Message = "Login Successful";

                        // Remove password before returning
                        user.Password = null;
                        service.Data = user;
                    }
                }
            }
            catch (Exception ex)
            {
                service.Status = 500;
                service.Success = false;
                service.Message = "Something went wrong: " + ex.Message;
                service.Data = null;
            }

            return service;
        }
    }
}