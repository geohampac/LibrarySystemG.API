using System.Threading.Tasks;
using LibrarySystemG.API.Model;

namespace LibrarySystemG.API.IRepository
{
    public interface IAdminRepository
    {
        Task<object> AdminLogin(AdminModel admin);
    }
}