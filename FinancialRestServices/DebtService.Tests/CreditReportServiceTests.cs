using DebtService.Data.Entities;
using DebtService.Data.Services;
using Flurl.Http;
using Flurl.Http.Configuration;
using Flurl.Http.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace DebtService.Tests
{
    public class CreditReportServiceTests
    {
        [Fact]
        public async Task GetCreditReportTradeLines_ReturnsSuccessfully()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<CreditReportService>>();

            var baseUrl = "https://myservice.com";
            var source = "DEF";
            var bureau = "XYZ";
            var inMemorySettings = new Dictionary<string, string> {
                {CreditReportService.BaseUrlConfigKey, baseUrl},
                {CreditReportService.SourceConfigKey, source},
                {CreditReportService.BureauConfigKey, bureau}
            };
            IConfiguration mockConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var applicationId = 5618358;

            var mockTradeLines = new List<TradeLine>
            {
                new TradeLine
                {
                    accountNumber = "31231232321",
                    balance = 500,
                    monthlyPayment = 30,
                    isMortgage = false,
                    tradelineId = 2312321312454675L,
                    type = "UNSECURED"
                }
            };
            var mockCreditData = new CreditData
            {
                applicationId = applicationId,
                customerName = "George Jetson",
                bureau = bureau,
                source = source,
                tradeLines = mockTradeLines
            };

            var service = new CreditReportService(mockLogger.Object, new DefaultFlurlClientFactory(), mockConfiguration);

            //Act
            IEnumerable<TradeLine> svcResponse;
            using (var httpTest = new HttpTest())
            {
                //Mock the external service request
                httpTest.ForCallsTo($"{baseUrl}/Credit/Report")
                    .WithQueryParam("applicationId", applicationId)
                    .WithQueryParam("source", source)
                    .WithQueryParam("bureau", bureau)
                    .RespondWithJson(mockCreditData);

                //Make the call
                svcResponse = await service.GetCreditReportTradeLines(applicationId);

                //Assert fake client
                httpTest.ShouldHaveCalled($"{baseUrl}/Credit/Report").Times(1);
            }

            //Assert
            Assert.NotNull(svcResponse);
            Assert.Single(svcResponse);

            Assert.Equal(mockTradeLines.First().accountNumber, svcResponse.First().accountNumber);
            Assert.Equal(mockTradeLines.First().balance, svcResponse.First().balance);
            Assert.Equal(mockTradeLines.First().monthlyPayment, svcResponse.First().monthlyPayment);
            Assert.Equal(mockTradeLines.First().isMortgage, svcResponse.First().isMortgage);
            Assert.Equal(mockTradeLines.First().tradelineId, svcResponse.First().tradelineId);
            Assert.Equal(mockTradeLines.First().type, svcResponse.First().type);
        }

        [Fact]
        public async Task GetCreditReportTradeLines_ThrowsWhenHttpStatusError()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<CreditReportService>>();

            var baseUrl = "https://myservice.com";
            var source = "DEF";
            var bureau = "XYZ";
            var inMemorySettings = new Dictionary<string, string> {
                {CreditReportService.BaseUrlConfigKey, baseUrl},
                {CreditReportService.SourceConfigKey, source},
                {CreditReportService.BureauConfigKey, bureau}
            };
            IConfiguration mockConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var applicationId = 5618358;
            var service = new CreditReportService(mockLogger.Object, new DefaultFlurlClientFactory(), mockConfiguration);

            //Act/Assert
            IEnumerable<TradeLine> svcResponse;
            using (var httpTest = new HttpTest())
            {
                //Mock the external service request
                httpTest.ForCallsTo($"{baseUrl}/Credit/Report")
                    .WithQueryParam("applicationId", applicationId)
                    .WithQueryParam("source", source)
                    .WithQueryParam("bureau", bureau)
                    .RespondWith("Some error", 500);

                //Make the call
                try
                {
                    svcResponse = await service.GetCreditReportTradeLines(applicationId);
                }
                catch(FlurlHttpException ex)
                {
                    Assert.NotNull(ex);
                    return;
                }
                //throw test if the exception type isn't http based 
                catch(Exception unexpected)
                {
                    Assert.True(false);
                }
            }

            //fail-safe
            Assert.True(false);
        }
    }
}