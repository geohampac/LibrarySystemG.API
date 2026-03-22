using LibrarySystemG.API.Model.Request;
using LibrarySystemG.API.Model.Response;
using System.Threading.Tasks;

namespace LibrarySystemG.API.IRepository  // ✅ capital R
{
    public interface ILoginRepository
    {
        // Must match exactly the method in LoginClass
        Task<ServiceResponse<LoginResponseModel>> Login(LoginModel model);
    }
}