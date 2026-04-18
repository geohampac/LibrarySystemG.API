using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using LibrarySystemG.API.IRepository;
using LibrarySystemG.API.Model.Request;
using LibrarySystemG.API.Model.Response;
using Microsoft.Extensions.Configuration;

namespace LibrarySystemG.API.Class
{
    public class BookClass : IBookRepository
    {
        private readonly string _connectionString;

        public BookClass(IConfiguration config)
        {
            // ✅ FIXED: matches your appsettings.json key exactly
            _connectionString = config.GetConnectionString("trackerlibrary")!;
        }

        // ── INSERT ────────────────────────────────────────────────────────────
        public async Task<ServiceResponse<object>> InsertBook(BookModel model)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SP_LIBRARYSYSTEM_MANAGEBOOK", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@Author", model.Author);
                cmd.Parameters.AddWithValue("@Genre", model.Genre);
                cmd.Parameters.AddWithValue("@Quantity", model.Quantity);
                cmd.Parameters.AddWithValue("@statementType", "Insert");

                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return Success("Book inserted.", model);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        // ── UPDATE ────────────────────────────────────────────────────────────
        public async Task<ServiceResponse<object>> UpdateBook(BookModel model)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SP_LIBRARYSYSTEM_MANAGEBOOK", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@BookID", model.BookID);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@Author", model.Author);
                cmd.Parameters.AddWithValue("@Genre", model.Genre);
                cmd.Parameters.AddWithValue("@Quantity", model.Quantity);
                cmd.Parameters.AddWithValue("@statementType", "Update");

                await con.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                BookModel? updated = null;
                if (await reader.ReadAsync())
                    updated = MapBook(reader);

                return updated != null
                    ? Success("Book updated.", updated)
                    : NotFound("Book not found.");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        public async Task<ServiceResponse<object>> DeleteBook(int id)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SP_LIBRARYSYSTEM_MANAGEBOOK", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@BookID", id);
                cmd.Parameters.AddWithValue("@statementType", "Delete");

                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return Success("Book deleted.");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        // ── GET ALL ───────────────────────────────────────────────────────────
        public async Task<ServiceResponse<object>> GetAllBooks()
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SP_LIBRARYSYSTEM_MANAGEBOOK", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@statementType", "GetAll");

                await con.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                var books = new List<BookModel>();
                while (await reader.ReadAsync())
                    books.Add(MapBook(reader));

                return Success("Books retrieved.", books);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        // ── GET BY ID ─────────────────────────────────────────────────────────
        public async Task<ServiceResponse<object>> GetBookById(int id)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SP_LIBRARYSYSTEM_MANAGEBOOK", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@statementType", "GetAll");

                await con.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var book = MapBook(reader);
                    if (book.BookID == id)
                        return Success("Book found.", book);
                }

                return NotFound("Book not found.");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        // ── Map reader row → BookModel ────────────────────────────────────────
        private static BookModel MapBook(SqlDataReader reader) => new BookModel
        {
            BookID = reader.GetInt32(reader.GetOrdinal("BookID")),
            Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? "" : reader.GetString(reader.GetOrdinal("Title")),
            Author = reader.IsDBNull(reader.GetOrdinal("Author")) ? "" : reader.GetString(reader.GetOrdinal("Author")),
            Genre = reader.IsDBNull(reader.GetOrdinal("Genre")) ? "" : reader.GetString(reader.GetOrdinal("Genre")),
            Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 0 : reader.GetInt32(reader.GetOrdinal("Quantity")),
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