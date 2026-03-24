using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibrarySystemG.API.IRepository;
using LibrarySystemG.API.Model.Request;
using LibrarySystemG.API.Model.Response;

namespace LibrarySystemG.API.Class
{
    public class BookClass : IBookRepository
    {
        private static readonly List<BookModel> _books = new();
        private static int _nextId = 1;

        public async Task<ServiceResponse<object>> InsertBook(BookModel model)
        {
            await Task.Delay(50);

            model.BookID = _nextId++;
            _books.Add(model);

            return Success("Book inserted", model);
        }

        public async Task<ServiceResponse<object>> UpdateBook(BookModel model)
        {
            await Task.Delay(50);

            var book = _books.FirstOrDefault(b => b.BookID == model.BookID);
            if (book == null)
                return NotFound("Book not found");

            book.Title = model.Title;
            book.Author = model.Author;
            book.Genre = model.Genre;
            book.Quantity = model.Quantity;

            return Success("Book updated", book);
        }

        public async Task<ServiceResponse<object>> DeleteBook(int id)
        {
            await Task.Delay(50);

            var book = _books.FirstOrDefault(b => b.BookID == id);
            if (book == null)
                return NotFound("Book not found");

            _books.Remove(book);
            return Success("Book deleted");
        }

        public async Task<ServiceResponse<object>> GetAllBooks()
        {
            await Task.Delay(50);
            return Success("Books retrieved", _books);
        }

        public async Task<ServiceResponse<object>> GetBookById(int id)
        {
            await Task.Delay(50);

            var book = _books.FirstOrDefault(b => b.BookID == id);
            return book == null
                ? NotFound("Book not found")
                : Success("Book found", book);
        }

        // 🔥 Helper Methods (Cleaner Code)
        private ServiceResponse<object> Success(string message, object data = null)
        {
            return new ServiceResponse<object>
            {
                Status = 200,
                Success = true,
                Message = message,
                Data = data
            };
        }

        private ServiceResponse<object> NotFound(string message)
        {
            return new ServiceResponse<object>
            {
                Status = 404,
                Success = false,
                Message = message
            };
        }
    }
}