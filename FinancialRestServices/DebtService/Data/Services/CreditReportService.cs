using DebtService.Data.Entities;
using DebtService.Data.Interfaces;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace DebtService.Data.Services
{
    public class CreditReportService : ICreditReportService
    {
        public const string BaseUrlConfigKey = "CreditReportServiceBaseUrl";
        public const string SourceConfigKey = "ServiceSource";
        public const string BureauConfigKey = "ServiceBureau";

        private readonly ILogger<CreditReportService> _logger;
        private readonly IFlurlClient _flurlClient;
        private readonly IConfiguration _configuration;

        public CreditReportService(ILogger<CreditReportService> logger, IFlurlClientFactory flurlClientFac, IConfiguration configuration)
        {
            _logger = logger;

            //Grab app settings information to configure the HTTP client.
            _configuration = configuration;
            var serviceBaseUrl = _configuration.GetValue<string>(BaseUrlConfigKey);
            var serviceSource = _configuration.GetValue<string>(SourceConfigKey);
            var serviceBureau = _configuration.GetValue<string>(BureauConfigKey);

            _flurlClient = flurlClientFac.Get(serviceBaseUrl);
            _flurlClient.BaseUrl = serviceBaseUrl;
            //Point Flurl client at Credit Report Service and set the source and bureaus to be added/overridden on each call
            //Due to requirement of locking down those parameters (Though they are pulled from app settings - not strictly hard coded).
            _flurlClient.BeforeCall(call => call.Request.SetQueryParam("source", serviceSource).SetQueryParam("bureau", serviceBureau));
        }

        /// </inheritdoc>
        public async Task<IEnumerable<TradeLine>> GetCreditReportTradeLines(int applicationId)
        {
            _logger.LogDebug($"{nameof(CreditReportService)}.{nameof(GetCreditReportTradeLines)} - Parameters " +
                $"{nameof(applicationId)}={applicationId}");

            //Configure request
            var request = _flurlClient
                .Request("Credit", "Report")
                .SetQueryParam("applicationId", applicationId);

            //Make request and try to serialize to the Credit Data type before retrieving just the tradelines.
            try
            {
                var svcResponse = await request.GetJsonAsync<CreditData>();
                return svcResponse.tradeLines;
            }
            catch(FlurlHttpException httpErr)
            {
                //Log HTTP exceptions with body
                string responseBody = await httpErr.GetResponseStringAsync();
                _logger.LogError($"{nameof(CreditReportService)}.{nameof(GetCreditReportTradeLines)} - Non 2xx from Credit Report Service" +
                    $"Response Body: {responseBody}");
                //Throw original stack info
                throw;
            }
        }
    }
}
