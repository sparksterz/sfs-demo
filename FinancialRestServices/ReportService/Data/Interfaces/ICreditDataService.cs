using JsonFlatFileDataStore;
using ReportService.Data.Entities;

namespace ReportService.Data.Services
{
    public interface ICreditDataService
    {
        /// <summary>
        /// Fetches a credit report from the backing datastore
        /// </summary>
        /// <param name="applicationId">Id of the credit report application</param>
        /// <param name="source">The source which originated the application</param>
        /// <param name="bureau">The credit bureau to retrieve data from</param>
        /// <returns>Will return either a single report if found, null report if not, and one of the exceptions on error</returns>
        /// <exception cref="UnexpectedMultipleItemException">Will return when multiple records are found</exception>
        CreditData GetCreditReport(int applicationId, string source, string bureau);
    }
}
