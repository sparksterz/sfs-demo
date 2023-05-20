using JsonFlatFileDataStore;
using ReportService.Data.Entities;
using ReportService.Data.Exceptions;

namespace ReportService.Data.Services
{
    public class CreditDataService : ICreditDataService
    {
        private readonly ILogger<CreditDataService> _logger;
        private readonly IDataStore _creditDataStore;
        private readonly string _config_seed_data_root;

        public const string SeedDataRootConfigKey = "SeedDataRoot";

        public CreditDataService(ILogger<CreditDataService> logger, IDataStore creditDataStore, IConfiguration configuration)
        {
            _logger = logger;
            _creditDataStore = creditDataStore;
            _config_seed_data_root = configuration.GetValue<string>(SeedDataRootConfigKey);
        }

        /// </inheritdoc>
        public CreditData GetCreditReport(int applicationId, string source, string bureau)
        {
            _logger.LogDebug($"{nameof(CreditDataService)}.{nameof(GetCreditReport)}: Parameters" +
                $"{nameof(applicationId)}={applicationId}, {nameof(source)}={source}, {nameof(bureau)}={bureau}");
            
            //Retrieve Credit Reports Document
            var reportCollection = _creditDataStore.GetCollection<CreditData>(_config_seed_data_root);

            //Find reports specific to input criteria
            var result = reportCollection.Find(report =>
                report.applicationId == applicationId &&
                report.source == source &&
                report.bureau == bureau);

            if (result == null)
                _logger.LogWarning($"{nameof(CreditDataService)}.{nameof(GetCreditReport)}: Report not found\n" +
                    $"Parameters: {nameof(applicationId)}={applicationId}, {nameof(source)}={source}, {nameof(bureau)}={bureau}");
            else if (result.Count() > 1)
                throw new UnexpectedMultipleItemException($"{nameof(CreditDataService)}.{nameof(GetCreditReport)}: Multiple reports found\n" +
                    $"Parameters: {nameof(applicationId)}={applicationId}, {nameof(source)}={source}, {nameof(bureau)}={bureau}");

            return result?.FirstOrDefault();
        }
    }
}
