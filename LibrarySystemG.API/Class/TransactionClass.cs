using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using LibrarySystemG.API.IRepository;
using LibrarySystemG.API.Model.Response;
using LibrarySystemG.API.Models;
using Microsoft.Extensions.Configuration;

namespace LibrarySystemG.API.Class
{
    public class TransactionClass : ITransactionRepository
    {
        private readonly string _connectionString;

        public TransactionClass(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("trackerlibrary")!;
        }

        // ── BORROW ────────────────────────────────────────────────────────────
        public async Task<ServiceResponse<object>> BorrowBook(TransactionModel transaction)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                await con.OpenAsync();

                // Step 1: Check book exists and has stock
                using (var checkCmd = new SqlCommand(
                    "SELECT Quantity FROM Books WHERE BookID = @BookID", con))
                {
                    checkCmd.Parameters.AddWithValue("@BookID", transaction.BookID);
                    using var reader = await checkCmd.ExecuteReaderAsync();

                    if (!await reader.ReadAsync())
                        return NotFound($"Book with ID {transaction.BookID} not found in database.");

                    var qty = reader.GetInt32(0);
                    if (qty <= 0)
                        return NotFound("Book is currently unavailable (quantity is 0).");
                }

                // Step 2: Call SP Insert — returns SCOPE_IDENTITY() as TransactionID
                int newTransactionId = 0;

                using var cmd = new SqlCommand("SP_LIBRARYSYSTEM_MANAGETRANSACTION", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@TransactionID", DBNull.Value);
                cmd.Parameters.AddWithValue("@StudentID", transaction.StudentID);
                cmd.Parameters.AddWithValue("@BookID", transaction.BookID);
                cmd.Parameters.AddWithValue("@BorrowDate", transaction.BorrowDate ?? (object)DateTime.Today);
                cmd.Parameters.AddWithValue("@ReturnDate", transaction.ReturnDate ?? (object)DateTime.Today.AddDays(14));
                cmd.Parameters.AddWithValue("@statementType", "Insert");

                using var txnReader = await cmd.ExecuteReaderAsync();

                // ✅ Read the TransactionID returned by SCOPE_IDENTITY()
                if (await txnReader.ReadAsync())
                {
                    newTransactionId = Convert.ToInt32(txnReader["TransactionID"]);
                }

                return Success("Book borrowed successfully.", new
                {
                    TransactionID = newTransactionId,   // ✅ real ID from DB
                    transaction.StudentID,
                    transaction.BookID,
                    transaction.BorrowDate,
                    transaction.ReturnDate
                });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        // ── RETURN ────────────────────────────────────────────────────────────
        public async Task<ServiceResponse<object>> ReturnBook(TransactionModel transaction)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                await con.OpenAsync();

                using var cmd = new SqlCommand("SP_LIBRARYSYSTEM_MANAGETRANSACTION", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@TransactionID", transaction.TransactionID);
                cmd.Parameters.AddWithValue("@StudentID", DBNull.Value);
                cmd.Parameters.AddWithValue("@BookID", transaction.BookID);
                cmd.Parameters.AddWithValue("@BorrowDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@ReturnDate", transaction.ReturnDate ?? (object)DateTime.Today);
                cmd.Parameters.AddWithValue("@statementType", "Return");

                using var reader = await cmd.ExecuteReaderAsync();

                return Success("Book returned successfully.", transaction.TransactionID);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        // ── GET ALL ───────────────────────────────────────────────────────────
        public async Task<ServiceResponse<object>> GetAllTransactions()
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SELECT * FROM Transactions", con);

                await con.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                var list = new List<TransactionModel>();
                while (await reader.ReadAsync())
                    list.Add(MapTransaction(reader));

                return Success("Transactions retrieved.", list);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        // ── GET BY ID ─────────────────────────────────────────────────────────
        public async Task<ServiceResponse<object>> GetTransactionById(int id)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(
                    "SELECT * FROM Transactions WHERE TransactionID = @ID", con);
                cmd.Parameters.AddWithValue("@ID", id);

                await con.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                    return Success("Transaction found.", MapTransaction(reader));

                return NotFound("Transaction not found.");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        public async Task<ServiceResponse<object>> DeleteTransaction(int transactionId)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(
                    "DELETE FROM Transactions WHERE TransactionID = @ID", con);
                cmd.Parameters.AddWithValue("@ID", transactionId);

                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return Success("Transaction deleted.");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        // ── Map reader → TransactionModel ─────────────────────────────────────
        private static TransactionModel MapTransaction(SqlDataReader r) => new()
        {
            TransactionID = r.GetInt32(r.GetOrdinal("TransactionID")),
            StudentID = r.IsDBNull(r.GetOrdinal("StudentID")) ? 0 : r.GetInt32(r.GetOrdinal("StudentID")),
            BookID = r.IsDBNull(r.GetOrdinal("BookID")) ? 0 : r.GetInt32(r.GetOrdinal("BookID")),
            BorrowDate = r.IsDBNull(r.GetOrdinal("BorrowDate")) ? null : r.GetDateTime(r.GetOrdinal("BorrowDate")),
            ReturnDate = r.IsDBNull(r.GetOrdinal("ReturnDate")) ? null : r.GetDateTime(r.GetOrdinal("ReturnDate")),
        };

        // ── Helpers ───────────────────────────────────────────────────────────
        private ServiceResponse<object> Success(string message, object data = null)
            => new() { Status = 200, Success = true, Message = message, Data = data };

        private ServiceResponse<object> NotFound(string message)
            => new() { Status = 404, Success = false, Message = message };

        private ServiceResponse<object> Error(string message)
            => new() { Status = 500, Success = false, Message = message };
    }
}