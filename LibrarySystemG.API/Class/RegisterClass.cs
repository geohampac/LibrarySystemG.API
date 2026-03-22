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
            var service = new ServiceResponse<RegisterResponseModel>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var param = new DynamicParameters();

                    param.Add("@FirstName", model.FirstName);
                    param.Add("@LastName", model.LastName);
                    param.Add("@Username", model.Username);
                    param.Add("@Password", model.Password);
                    param.Add("@Course", model.Course);
                    param.Add("@Email", model.Email);

                    var result = await conn.QueryFirstOrDefaultAsync<RegisterResponseModel>(
                        "LIBRARYSYSTEM_REGISTER",
                        param,
                        commandType: CommandType.StoredProcedure
                    );

                    if (result == null)
                    {
                        service.Status = 400;
                        service.Success = false;
                        service.Message = "Registration failed";
                        service.Data = null;
                    }
                    else
                    {
                        service.Status = 200;
                        service.Success = true;
                        service.Message = result.Message;
                        service.Data = result;
                    }
                }
            }
            catch (Exception ex)
            {
                service.Status = 500;
                service.Success = false;
                service.Message = ex.Message;
                service.Data = null;
            }

            return service;
        }
    }
}