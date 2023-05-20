using DebtService.Data.Entities;

namespace DebtService.Data.Interfaces
{
    public interface ICreditReportService
    {
        /// <summary>
        /// This method is intended to contact ReportService - Service 1 to retrieve the
        /// credit report for the given application id. From there it returns just the
        /// tradeline data
        /// </summary>
        /// <param name="applicationId">Credit report application id</param>
        /// <returns>Tradeline data for the given credit report</returns>
        /// <exception cref="FlurlHttpException">This exception throws on any non 200 result</exception>
        Task<IEnumerable<TradeLine>> GetCreditReportTradeLines(int applicationId);
    }
}
