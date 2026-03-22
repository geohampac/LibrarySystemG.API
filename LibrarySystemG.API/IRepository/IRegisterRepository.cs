using LibrarySystemG.API.Model;
using LibrarySystemG.API.Model.Response;

namespace LibrarySystemG.API.IRepository  // <-- capital "R" matches your class using statement
{
    public interface IRegisterRepository
    {
        // Use the same type your RegisterClass returns
        Task<ServiceResponse<RegisterResponseModel>> Register(RegisterModel model);
    }
}