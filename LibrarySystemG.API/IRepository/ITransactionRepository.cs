using LibrarySystemG.API.Model;
using LibrarySystemG.API.Model.Response;
using LibrarySystemG.API.Models;

namespace LibrarySystemG.API.IRepository   // ✅ consistent
{
    public interface ITransactionRepository
    {
        Task<ServiceResponse<object>> BorrowBook(TransactionModel transaction);
        Task<ServiceResponse<object>> ReturnBook(TransactionModel transaction);
        Task<ServiceResponse<object>> DeleteTransaction(int transactionId);
        Task<ServiceResponse<object>> GetAllTransactions();
        Task<ServiceResponse<object>> GetTransactionById(int id);
    }
}