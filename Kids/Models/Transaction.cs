
namespace Kids.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FactorId { get; set; }
        public DateTime ActionDate { get; set; }
        public string UserIP { get; set; }
        public string BankCallBackId { get; set; }
        public decimal Price { get; set; }
      

    }
}
