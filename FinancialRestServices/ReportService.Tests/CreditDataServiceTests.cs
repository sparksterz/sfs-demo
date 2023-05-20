using JsonFlatFileDataStore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using ReportService.Data.Entities;
using ReportService.Data.Exceptions;
using ReportService.Data.Services;

namespace ReportService.Tests
{
    public class CreditDataServiceTests
    {
        [Fact]
        public void GetCreditReport_ReturnsSuccessfully()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<CreditDataService>>();
            
            var mockDataStore = new Mock<IDataStore>();
            var mockDocumentCollection = new Mock<IDocumentCollection<CreditData>>();
            var inMemorySettings = new Dictionary<string, string> {
                {CreditDataService.SeedDataRootConfigKey, ""}
            };
            IConfiguration mockConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();


            var mockAppId = 123;
            var mockSource = "DEF";
            var mockBureau = "ETF";

            CreditData entityResponse = new CreditData
            {
                applicationId = mockAppId,
                source = mockSource,
                bureau = mockBureau
            };

            mockDataStore.Setup(x => x.GetCollection<CreditData>(It.IsAny<string>())).Returns(mockDocumentCollection.Object);
            mockDocumentCollection.Setup(x => x.Find(It.IsAny<Predicate<CreditData>>())).Returns(new List<CreditData>(){ entityResponse });

            CreditDataService creditDataService = new CreditDataService(mockLogger.Object, mockDataStore.Object, mockConfiguration);

            //Act
            var result = creditDataService.GetCreditReport(mockAppId, mockSource, mockBureau);

            //Assert
            Assert.NotNull(result);

            Assert.Equal(entityResponse.applicationId, result.applicationId);
            Assert.Same(entityResponse.source, result.source);
            Assert.Same(entityResponse.bureau, result.bureau);

            mockDataStore.VerifyAll();
            mockDocumentCollection.VerifyAll();
        }

        [Fact]
        public void GetCreditReport_HandlesNullResults_Successfully()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<CreditDataService>>();

            var mockDataStore = new Mock<IDataStore>();
            var mockDocumentCollection = new Mock<IDocumentCollection<CreditData>>();
            var inMemorySettings = new Dictionary<string, string> {
                {CreditDataService.SeedDataRootConfigKey, ""}
            };
            IConfiguration mockConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();


            var mockAppId = 123;
            var mockSource = "DEF";
            var mockBureau = "ETF";

            CreditData entityResponse = new CreditData
            {
                applicationId = mockAppId,
                source = mockSource,
                bureau = mockBureau
            };

            mockDataStore.Setup(x => x.GetCollection<CreditData>(It.IsAny<string>())).Returns(mockDocumentCollection.Object);
            mockDocumentCollection.Setup(x => x.Find(It.IsAny<Predicate<CreditData>>())).Returns((IEnumerable<CreditData>)null);

            CreditDataService creditDataService = new CreditDataService(mockLogger.Object, mockDataStore.Object, mockConfiguration);

            //Act
            var result = creditDataService.GetCreditReport(mockAppId, mockSource, mockBureau);

            //Assert
            Assert.Null(result);

            mockDataStore.VerifyAll();
            mockDocumentCollection.VerifyAll();
        }

        [Fact]
        public void GetCreditReport_HandlesMultipleResults_WithException()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<CreditDataService>>();

            var mockDataStore = new Mock<IDataStore>();
            var mockDocumentCollection = new Mock<IDocumentCollection<CreditData>>();
            var inMemorySettings = new Dictionary<string, string> {
                {CreditDataService.SeedDataRootConfigKey, ""}
            };
            IConfiguration mockConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var mockAppId = 123;
            var mockSource = "DEF";
            var mockBureau = "ETF";

            CreditData entityResponse = new CreditData
            {
                applicationId = mockAppId,
                source = mockSource,
                bureau = mockBureau
            };

            mockDataStore.Setup(x => x.GetCollection<CreditData>(It.IsAny<string>())).Returns(mockDocumentCollection.Object);
            mockDocumentCollection.Setup(x => x.Find(It.IsAny<Predicate<CreditData>>()))
                .Returns(new List<CreditData>() { entityResponse, entityResponse });

            CreditDataService creditDataService = new CreditDataService(mockLogger.Object, mockDataStore.Object, mockConfiguration);

            //Act/Assert
            try
            {
                var result = creditDataService.GetCreditReport(mockAppId, mockSource, mockBureau);
                Assert.True(false);
            }
            catch(UnexpectedMultipleItemException unex)
            {
                Assert.True(!string.IsNullOrEmpty(unex.Message));

                mockDataStore.VerifyAll();
                mockDocumentCollection.VerifyAll();
            }
        }
    }
}