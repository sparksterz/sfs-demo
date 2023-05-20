using DebtService.Business.Services;
using DebtService.Controllers;
using DebtService.Data.Entities;
using DebtService.Data.Interfaces;
using DebtService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DebtService.Tests
{
    public class StatisticsControllerTests
    {
        [Fact]
        public async Task GetDebts_ReturnsSuccesfully()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<StatisticsController>>();
            var mockCreditReportService = new Mock<ICreditReportService>(MockBehavior.Strict);
            var mockStatisticsCalculationService = new Mock<IStatisticCalculationService>(MockBehavior.Strict);

            var applicationId = 5618358;
            var annualIncome = 50000;

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
                bureau = "ABC",
                source = "123",
                tradeLines = mockTradeLines
            };
            var expectedDebtStats = new DebtStatistics()
            {
                DebtToIncome = 0.12m,
                NumberOfUnsecuredTradeLines = 1,
                UnsecuredDebtBalance = 500
            };

            mockCreditReportService.Setup(x => x.GetCreditReportTradeLines(It.Is<int>(a => a == applicationId)))
                .ReturnsAsync(mockCreditData.tradeLines);
            mockStatisticsCalculationService
                .Setup(x => x.CalculateDebtStatistics(
                    It.Is<IEnumerable<TradeLine>>(a => a.Single().tradelineId == mockTradeLines.Single().tradelineId),
                    It.Is<int>(b => b == annualIncome)))
                .Returns(expectedDebtStats);

            var controller = new StatisticsController(mockLogger.Object, mockCreditReportService.Object, mockStatisticsCalculationService.Object);

            //Act
            var result = await controller.GetDebts(applicationId, annualIncome);

            //Assert
            Assert.NotNull(result);
            var debtStats = (DebtStatistics)((OkObjectResult)result).Value;
            Assert.NotNull(debtStats);

            Assert.Equal(expectedDebtStats.DebtToIncome, debtStats.DebtToIncome);
            Assert.Equal(expectedDebtStats.NumberOfUnsecuredTradeLines, debtStats.NumberOfUnsecuredTradeLines);
            Assert.Equal(expectedDebtStats.UnsecuredDebtBalance, debtStats.UnsecuredDebtBalance);

            mockCreditReportService.VerifyAll();
            mockStatisticsCalculationService.VerifyAll();
        }

        [Fact]
        public async Task GetDebts_Returns500StatusCodeResult_OnCreditReportServiceException()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<StatisticsController>>();
            var mockCreditReportService = new Mock<ICreditReportService>(MockBehavior.Strict);
            var mockStatisticsCalculationService = new Mock<IStatisticCalculationService>(MockBehavior.Strict);

            var applicationId = 5618358;
            var annualIncome = 50000;

            mockCreditReportService.Setup(x => x.GetCreditReportTradeLines(It.Is<int>(a => a == applicationId)))
                .Throws(new Exception("Test Error"));

            var controller = new StatisticsController(mockLogger.Object, mockCreditReportService.Object, mockStatisticsCalculationService.Object);

            //Act
            var response = await controller.GetDebts(applicationId, annualIncome);

            //Assert
            Assert.NotNull(response);
            var customResponse = (ObjectResult)response;
            Assert.NotNull(customResponse);
            Assert.Equal(500, customResponse.StatusCode);

            mockCreditReportService.VerifyAll();
            mockStatisticsCalculationService.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetDebts_Returns500StatusCodeResult_OnCalculateDebtStatisticsException()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<StatisticsController>>();
            var mockCreditReportService = new Mock<ICreditReportService>(MockBehavior.Strict);
            var mockStatisticsCalculationService = new Mock<IStatisticCalculationService>(MockBehavior.Strict);

            var applicationId = 5618358;
            var annualIncome = 50000;

            mockCreditReportService.Setup(x => x.GetCreditReportTradeLines(It.Is<int>(a => a == applicationId)))
                .ReturnsAsync(new List<TradeLine>());
            mockStatisticsCalculationService.Setup(x => x.CalculateDebtStatistics(It.IsAny<IEnumerable<TradeLine>>(), It.Is<int>(a => a == annualIncome)))
                .Throws(new Exception("Test Error"));

            var controller = new StatisticsController(mockLogger.Object, mockCreditReportService.Object, mockStatisticsCalculationService.Object);

            //Act
            var response = await controller.GetDebts(applicationId, annualIncome);

            //Assert
            Assert.NotNull(response);
            var customResponse = (ObjectResult)response;
            Assert.NotNull(customResponse);
            Assert.Equal(500, customResponse.StatusCode);

            mockCreditReportService.VerifyAll();
            mockStatisticsCalculationService.VerifyAll();
        }
    }
}