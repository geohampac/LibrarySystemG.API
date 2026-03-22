using System.Threading.Tasks;
using LibrarySystemG.API.IRepository;
using LibrarySystemG.API.Model.Request;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemG.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BookController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpPost("insert")]
        public async Task<IActionResult> InsertBook([FromBody] BookModel book)
        {
            var result = await _bookRepository.InsertBook(book);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateBook([FromBody] BookModel book)
        {
            if (book.BookID <= 0)
                return BadRequest(new { Message = "Valid BookID is required" });

            var result = await _bookRepository.UpdateBook(book);
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _bookRepository.DeleteBook(id);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllBooks()
        {
            var result = await _bookRepository.GetAllBooks();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var result = await _bookRepository.GetBookById(id);
            return Ok(result);
        }
    }
}