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
        private static readonly List<TransactionModel> _transactions = new List<TransactionModel>();
        private static int _nextId = 1;

        // Borrow a book
        public async Task<ServiceResponse<object>> BorrowBook(TransactionModel transaction)
        {
            await Task.Delay(50);
            transaction.TransactionID = _nextId++;
            transaction.Type = "Borrowed";
            _transactions.Add(transaction);

            return new ServiceResponse<object>
            {
                Status = 200,
                Success = true,
                Message = "Book borrowed",
                Data = transaction
            };
        }

        // Return a book
        public async Task<ServiceResponse<object>> ReturnBook(TransactionModel transaction)
        {
            await Task.Delay(50);
            transaction.TransactionID = _nextId++;
            transaction.Type = "Returned";
            _transactions.Add(transaction);

            return new ServiceResponse<object>
            {
                Status = 200,
                Success = true,
                Message = "Book returned",
                Data = transaction
            };
        }

        // Delete a transaction
        public async Task<ServiceResponse<object>> DeleteTransaction(int transactionId)
        {
            await Task.Delay(50);
            var t = _transactions.FirstOrDefault(x => x.TransactionID == transactionId);
            if (t != null)
            {
                _transactions.Remove(t);
                return new ServiceResponse<object>
                {
                    Status = 200,
                    Success = true,
                    Message = "Transaction deleted"
                };
            }

            return new ServiceResponse<object>
            {
                Status = 404,
                Success = false,
                Message = "Transaction not found"
            };
        }

        // Get all transactions
        public async Task<ServiceResponse<object>> GetAllTransactions()
        {
            await Task.Delay(50);
            return new ServiceResponse<object>
            {
                Status = 200,
                Success = true,
                Message = "Transactions retrieved",
                Data = _transactions
            };
        }

        // Get transaction by ID
        public async Task<ServiceResponse<object>> GetTransactionById(int id)
        {
            await Task.Delay(50);
            var t = _transactions.FirstOrDefault(x => x.TransactionID == id);
            if (t != null)
            {
                return new ServiceResponse<object>
                {
                    Status = 200,
                    Success = true,
                    Message = "Transaction found",
                    Data = t
                };
            }

            return new ServiceResponse<object>
            {
                Status = 404,
                Success = false,
                Message = "Transaction not found"
            };
        }
    }
}