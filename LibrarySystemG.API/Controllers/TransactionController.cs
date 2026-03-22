using LibrarySystemG.API.IRepository;
using LibrarySystemG.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemG.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        // BORROW
        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBook([FromBody] TransactionModel transaction)
        {
            var response = await _transactionRepository.BorrowBook(transaction);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // RETURN
        [HttpPut("return")]
        public async Task<IActionResult> ReturnBook([FromBody] TransactionModel transaction)
        {
            var response = await _transactionRepository.ReturnBook(transaction);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var response = await _transactionRepository.DeleteTransaction(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            var response = await _transactionRepository.GetAllTransactions();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var response = await _transactionRepository.GetTransactionById(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}