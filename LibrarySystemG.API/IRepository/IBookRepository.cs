using LibrarySystemG.API.Model.Request;
using LibrarySystemG.API.Model.Response;
using System.Threading.Tasks;

namespace LibrarySystemG.API.IRepository
{
    public interface IBookRepository
    {
        Task<ServiceResponse<object>> InsertBook(BookModel model);
        Task<ServiceResponse<object>> UpdateBook(BookModel model);
        Task<ServiceResponse<object>> DeleteBook(int id);
        Task<ServiceResponse<object>> GetAllBooks();
        Task<ServiceResponse<object>> GetBookById(int id);
    }
}