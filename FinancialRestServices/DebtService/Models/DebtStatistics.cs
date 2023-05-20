namespace DebtService.Models
{
    public class DebtStatistics
    {
        public int NumberOfUnsecuredTradeLines { get; set; }
        public int UnsecuredDebtBalance { get; set; }
        public decimal DebtToIncome { get; set; }
    }
}
