using System.Collections.Generic;
using System.Threading.Tasks;
using LibrarySystemG.API.IRepository;
using LibrarySystemG.API.Model.Request;
using LibrarySystemG.API.Model.Response;

namespace LibrarySystemG.API.Class
{
    public class BookClass : IBookRepository
    {
        private static readonly List<BookModel> _books = new List<BookModel>();
        private static int _nextId = 1; 

        
        public async Task<ServiceResponse<object>> InsertBook(BookModel model)
        {
            await Task.Delay(50);
            model.BookID = _nextId++;
            _books.Add(model);

            return new ServiceResponse<object>
            {
                Status = 200,
                Success = true,
                Message = "Book inserted",
                Data = model
            };
        }

        public async Task<ServiceResponse<object>> UpdateBook(BookModel model)
        {
            await Task.Delay(50);
            var book = _books.Find(b => b.BookID == model.BookID);
            if (book != null)
            {
                book.Title = model.Title;
                book.Author = model.Author;
                book.Genre = model.Genre;
                book.Quantity = model.Quantity;

                return new ServiceResponse<object>
                {
                    Status = 200,
                    Success = true,
                    Message = "Book updated",
                    Data = book
                };
            }

            return new ServiceResponse<object>
            {
                Status = 404,
                Success = false,
                Message = "Book not found"
            };
        }

        public async Task<ServiceResponse<object>> DeleteBook(int id)
        {
            await Task.Delay(50);
            var book = _books.Find(b => b.BookID == id);
            if (book != null)
            {
                _books.Remove(book);
                return new ServiceResponse<object>
                {
                    Status = 200,
                    Success = true,
                    Message = "Book deleted"
                };
            }

            return new ServiceResponse<object>
            {
                Status = 404,
                Success = false,
                Message = "Book not found"
            };
        }

        // Get all books
        public async Task<ServiceResponse<object>> GetAllBooks()
        {
            await Task.Delay(50);
            return new ServiceResponse<object>
            {
                Status = 200,
                Success = true,
                Message = "Books retrieved",
                Data = _books
            };
        }

        public async Task<ServiceResponse<object>> GetBookById(int id)
        {
            await Task.Delay(50);
            var book = _books.Find(b => b.BookID == id);
            if (book != null)
            {
                return new ServiceResponse<object>
                {
                    Status = 200,
                    Success = true,
                    Message = "Book found",
                    Data = book
                };
            }

            return new ServiceResponse<object>
            {
                Status = 404,
                Success = false,
                Message = "Book not found"
            };
        }
    }
}