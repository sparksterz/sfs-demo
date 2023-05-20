using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ReportService.Controllers;
using ReportService.Data.Entities;
using ReportService.Data.Exceptions;
using ReportService.Data.Services;

namespace ReportService.Tests
{
    public class CreditControllerTests
    {
        [Fact]
        public void GetReport_ReturnsSuccessfully()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<CreditController>>();
            var mockCreditDataService = new Mock<ICreditDataService>(MockBehavior.Strict);

            var mockAppId = 123;
            var mockSource = "DEF";
            var mockBureau = "ETF";

            CreditData entityResponse = new CreditData
            {
                applicationId = mockAppId,
                source = mockSource,
                bureau = mockBureau
            };

            mockCreditDataService
                .Setup(x => x.GetCreditReport(
                    It.Is<int>(a => a == mockAppId), 
                    It.Is<string>(b => b == mockSource), 
                    It.Is<string>(c => c == mockBureau)))
                .Returns(entityResponse);

            CreditController controller = new CreditController(mockLogger.Object, mockCreditDataService.Object);

            //Act
            var response = controller.GetReport(mockAppId, mockSource, mockBureau);

            //Assert
            Assert.NotNull(response);
            var creditResponse = (CreditData)((OkObjectResult)response).Value;
            Assert.NotNull(creditResponse);

            Assert.Equal(entityResponse.applicationId, creditResponse.applicationId);
            Assert.Same(entityResponse.source, creditResponse.source);
            Assert.Same(entityResponse.bureau, creditResponse.bureau);

            mockCreditDataService.VerifyAll();
        }

        [Fact]
        public void GetReport_ReturnsNullSuccessfully()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<CreditController>>();
            var mockCreditDataService = new Mock<ICreditDataService>(MockBehavior.Strict);

            var mockAppId = 123;
            var mockSource = "DEF";
            var mockBureau = "ETF";

            CreditData entityResponse = new CreditData
            {
                applicationId = mockAppId,
                source = mockSource,
                bureau = mockBureau
            };

            mockCreditDataService
                .Setup(x => x.GetCreditReport(
                    It.Is<int>(a => a == mockAppId),
                    It.Is<string>(b => b == mockSource),
                    It.Is<string>(c => c == mockBureau)))
                .Returns((CreditData)null);

            CreditController controller = new CreditController(mockLogger.Object, mockCreditDataService.Object);

            //Act
            var response = controller.GetReport(mockAppId, mockSource, mockBureau);

            //Assert
            Assert.NotNull(response);
            var creditResponse = (CreditData)((OkObjectResult)response).Value;
            Assert.Null(creditResponse);

            mockCreditDataService.VerifyAll();
        }

        [Fact]
        public void GetReport_FoundMultipleReports_ReturnsSanitizedError()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<CreditController>>();
            var mockCreditDataService = new Mock<ICreditDataService>(MockBehavior.Strict);

            var mockAppId = 123;
            var mockSource = "DEF";
            var mockBureau = "ETF";

            var innerExceptionMessage = "Test error";

            mockCreditDataService
                .Setup(x => x.GetCreditReport(
                    It.Is<int>(a => a == mockAppId),
                    It.Is<string>(b => b == mockSource),
                    It.Is<string>(c => c == mockBureau)))
                .Throws(new UnexpectedMultipleItemException(innerExceptionMessage));

            CreditController controller = new CreditController(mockLogger.Object, mockCreditDataService.Object);

            //Act
            var response = controller.GetReport(mockAppId, mockSource, mockBureau);

            //Assert
            Assert.NotNull(response);
            var customResponse = (ObjectResult)response;
            Assert.NotNull(customResponse);

            Assert.Equal(500, customResponse.StatusCode);
            Assert.NotNull(customResponse.Value);
            Assert.NotSame(innerExceptionMessage, customResponse.Value);

            mockCreditDataService.VerifyAll();
        }
    }
}