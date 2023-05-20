using DebtService.Data.Entities;
using DebtService.Models;

namespace DebtService.Business.Services
{
    public interface IStatisticCalculationService
    {
        DebtStatistics CalculateDebtStatistics(IEnumerable<TradeLine> tradeLines, int annualIncome);
    }
}
