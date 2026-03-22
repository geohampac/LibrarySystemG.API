namespace LibrarySystemG.API.Models
{
    public class TransactionModel
    {
        public int TransactionID { get; set; }
        public int StudentID { get; set; }
        public int BookID { get; set; }
        public DateTime? BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        
        public string Type { get; set; }
    }
}