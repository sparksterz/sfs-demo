namespace DebtService.Data.Entities
{
    public class TradeLine
    {
        public long tradelineId { get; set; }
        public string accountNumber { get; set; }
        public int balance { get; set; }
        public int monthlyPayment { get; set; }
        public string type { get; set; }
        public bool isMortgage { get; set; }
    }
}
