using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibrarySystemG.API.IRepository;
using LibrarySystemG.API.Model;
using LibrarySystemG.API.Model.Response;
using LibrarySystemG.API.Models;

namespace LibrarySystemG.API.Class
{
    public class TransactionClass : ITransactionRepository
    {
        private static readonly List<TransactionModel> _transactions = new();
        private static int _nextId = 1;

        public async Task<ServiceResponse<object>> BorrowBook(TransactionModel transaction)
        {
            await Task.Delay(50);

            transaction.TransactionID = _nextId++;
            transaction.Type = "Borrowed";

            _transactions.Add(transaction);

            return Success("Book borrowed", transaction);
        }

        public async Task<ServiceResponse<object>> ReturnBook(TransactionModel transaction)
        {
            await Task.Delay(50);

            transaction.TransactionID = _nextId++;
            transaction.Type = "Returned";

            _transactions.Add(transaction);

            return Success("Book returned", transaction);
        }

        public async Task<ServiceResponse<object>> DeleteTransaction(int transactionId)
        {
            await Task.Delay(50);

            var transaction = _transactions.FirstOrDefault(t => t.TransactionID == transactionId);
            if (transaction == null)
                return NotFound("Transaction not found");

            _transactions.Remove(transaction);
            return Success("Transaction deleted");
        }

        public async Task<ServiceResponse<object>> GetAllTransactions()
        {
            await Task.Delay(50);
            return Success("Transactions retrieved", _transactions);
        }

        public async Task<ServiceResponse<object>> GetTransactionById(int id)
        {
            await Task.Delay(50);

            var transaction = _transactions.FirstOrDefault(t => t.TransactionID == id);

            return transaction == null
                ? NotFound("Transaction not found")
                : Success("Transaction found", transaction);
        }

        // 🔥 Helper Methods (same as BookClass for consistency)
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