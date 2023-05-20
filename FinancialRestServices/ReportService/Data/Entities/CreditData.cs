namespace ReportService.Data.Entities
{
    public class CreditData
    {
        public int applicationId { get; set; }
        public string customerName { get; set; }
        public string source { get; set; }
        public string bureau { get; set; }
        
        //It seems this doesn't actually exist in the seed data - consider removal
        [Obsolete]
        public int minPaymentPercentage { get; set; }

        public IEnumerable<TradeLine> tradeLines { get; set; }
    }
}
