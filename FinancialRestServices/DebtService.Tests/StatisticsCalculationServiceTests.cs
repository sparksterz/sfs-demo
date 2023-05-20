using DebtService.Business.Services;
using DebtService.Data.Entities;

namespace DebtService.Tests
{
    public class StatisticCalculationServiceTests
    {
        [Fact]
        public void CalculateDebtStatistics_UnsecuredTradeLinesCount_Success()
        {
            //Arrange
            var annualIncome = 50000;
            var mockTradeLines = new List<TradeLine>
            {
                new TradeLine
                {
                    type = "UNSECURED"
                },
                new TradeLine
                {
                    type = "SECURED"
                },
                new TradeLine
                {
                    type = "UNSECURED"
                }
            };

            var svc = new StatisticCalculationService();

            //Act
            var stats = svc.CalculateDebtStatistics(mockTradeLines, annualIncome);

            //Assert
            Assert.NotNull(stats);
            Assert.Equal(2, stats.NumberOfUnsecuredTradeLines);
        }

        [Fact]
        public void CalculateDebtStatistics_UnsecuredTradeLinesCount_NullOrEmptyTradeline_Success()
        {
            //Arrange
            var annualIncome = 50000;
            var mockTradeLines = new List<TradeLine>();

            var svc = new StatisticCalculationService();

            //Act
            var stats_forEmpty = svc.CalculateDebtStatistics(mockTradeLines, annualIncome);
            var stats_forNull = svc.CalculateDebtStatistics(null, annualIncome);

            //Assert
            Assert.NotNull(stats_forEmpty);
            Assert.Equal(0, stats_forEmpty.NumberOfUnsecuredTradeLines);

            Assert.NotNull(stats_forNull);
            Assert.Equal(0, stats_forNull.NumberOfUnsecuredTradeLines);
        }

        [Fact]
        public void CalculateDebtStatistics_DebtToIncome_Success()
        {
            //Arrange
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
                },
                new TradeLine
                {
                    accountNumber = "12355677735",
                    balance = 1500,
                    monthlyPayment = 150,
                    isMortgage = false,
                    tradelineId = 1915161178141758L,
                    type = "SECURED"
                },
                new TradeLine
                {
                    accountNumber = "234155188021",
                    balance = 200000,
                    monthlyPayment = 2000,
                    isMortgage = true,
                    tradelineId = 878115618189166L,
                    type = "UNSECURED"
                }
            };

            var svc = new StatisticCalculationService();

            //Act
            var stats = svc.CalculateDebtStatistics(mockTradeLines, annualIncome);

            //Assert
            Assert.NotNull(stats);
            //Total monthly debt payments from secured or unsecured tradelines (skipping those that are mortgages)
            //divided by monthly income
            Assert.Equal(stats.DebtToIncome, (180/4166.66m), 6);
        }

        [Fact]
        public void CalculateDebtStatistics_DebtToIncome_NullOrEmptyTradelines()
        {
            //Arrange
            var annualIncome = 50000;
            var mockTradeLines = new List<TradeLine>();

            var svc = new StatisticCalculationService();

            //Act
            var stats_forNull = svc.CalculateDebtStatistics(mockTradeLines, annualIncome);
            var stats_forEmpty = svc.CalculateDebtStatistics(mockTradeLines, annualIncome);

            //Assert
            Assert.NotNull(stats_forEmpty);
            Assert.Equal(0, stats_forEmpty.DebtToIncome);

            Assert.NotNull(stats_forNull);
            Assert.Equal(0, stats_forNull.DebtToIncome);
        }

        [Fact]
        public void CalculateDebtStatistics_DebtToIncome_NoAnnualIncomeOrNegative_Throws()
        {
            //Arrange
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
                },
                new TradeLine
                {
                    accountNumber = "12355677735",
                    balance = 1500,
                    monthlyPayment = 150,
                    isMortgage = false,
                    tradelineId = 1915161178141758L,
                    type = "SECURED"
                },
                new TradeLine
                {
                    accountNumber = "234155188021",
                    balance = 200000,
                    monthlyPayment = 2000,
                    isMortgage = true,
                    tradelineId = 878115618189166L,
                    type = "UNSECURED"
                }
            };

            var svc = new StatisticCalculationService();

            bool expectedZeroResult = false;
            bool expectedNegativeResult = false;
            //Act
            try
            {
                var stats = svc.CalculateDebtStatistics(mockTradeLines, 0);
            }
            catch(ArgumentException ex)
            {
                expectedZeroResult = true;
            }
            try
            {
                var stats = svc.CalculateDebtStatistics(mockTradeLines, -1000);
            }
            catch (ArgumentException ex)
            {
                expectedNegativeResult = true;
            }

            //Assert
            Assert.True(expectedZeroResult);
            Assert.True(expectedNegativeResult);
        }

        [Fact]
        public void CalculateDebtStatistics_GetUnsecuredDebtBalance_Success()
        {
            //Arrange
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
                },
                new TradeLine
                {
                    accountNumber = "12355677735",
                    balance = 1500,
                    monthlyPayment = 150,
                    isMortgage = false,
                    tradelineId = 1915161178141758L,
                    type = "SECURED"
                },
                new TradeLine
                {
                    accountNumber = "234155188021",
                    balance = 200000,
                    monthlyPayment = 2000,
                    isMortgage = true,
                    tradelineId = 878115618189166L,
                    type = "UNSECURED"
                }
            };
            //Item 1 Balance + Item 3 Balance
            var expectedSum = 200000 + 500;

            var svc = new StatisticCalculationService();

            //Act
            var stats = svc.CalculateDebtStatistics(mockTradeLines, annualIncome);

            //Assert
            Assert.Equal(expectedSum, stats.UnsecuredDebtBalance);
        }

        [Fact]
        public void CalculateDebtStatistics_GetUnsecuredDebtBalance_NullOrEmptyTradelines_Success()
        {
            //Arrange
            var annualIncome = 50000;
            var mockTradeLines = new List<TradeLine>();

            var svc = new StatisticCalculationService();

            //Act
            var stats_forNull = svc.CalculateDebtStatistics(mockTradeLines, annualIncome);
            var stats_forEmpty = svc.CalculateDebtStatistics(mockTradeLines, annualIncome);

            //Assert
            Assert.NotNull(stats_forEmpty);
            Assert.Equal(0, stats_forEmpty.UnsecuredDebtBalance);

            Assert.NotNull(stats_forNull);
            Assert.Equal(0, stats_forNull.UnsecuredDebtBalance);
        }
    }
}