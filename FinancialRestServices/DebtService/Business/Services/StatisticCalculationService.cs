using DebtService.Data.Entities;
using DebtService.Models;

namespace DebtService.Business.Services
{
    /// <summary>
    /// This class is focused on being responsible for the business logic of calculating debt info based
    /// on tradelines.
    /// </summary>
    public class StatisticCalculationService : IStatisticCalculationService
    {
        private readonly string UNSECURED_TRADE_LINE_TYPE = "UNSECURED";
        private readonly string SECURED_TRADE_LINE_TYPE = "SECURED";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tradeLines"></param>
        /// <param name="annualIncome"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"
        public DebtStatistics CalculateDebtStatistics(IEnumerable<TradeLine> tradeLines, int annualIncome)
        {
            return new DebtStatistics
            {
                DebtToIncome = CalculateDebtToIncome(tradeLines, annualIncome),
                NumberOfUnsecuredTradeLines = CountUnsecuredTradeLines(tradeLines),
                UnsecuredDebtBalance = GetUnsecuredDebtBalance(tradeLines)
            };
        }

        /// <summary>
        /// Given the tradeline data input into the calulator, count the unsecured ones.
        /// </summary>
        /// <returns>If we have no tradelines it's 0, otherwise the total amount of unsecured tradelines.</returns>
        private int CountUnsecuredTradeLines(IEnumerable<TradeLine> tradeLines)
        {
            return (tradeLines != null && tradeLines.Count() > 0) ?
                tradeLines.Count(x => x.type == UNSECURED_TRADE_LINE_TYPE) : 0;
        }

        /// <summary>
        /// Calculates your debt to income percentage. It utilizes the sum of the monthly payments
        /// on all tradelines which are secured or unsecured, but ignores mortgage payments.
        /// </summary>
        /// <returns>A calculation of your monthly debts divided by monthly income</returns>
        /// <exception cref="ArgumentException">If you have no annual income, this is a bad calculation</exception>
        private decimal CalculateDebtToIncome(IEnumerable<TradeLine> tradeLines, int annualIncome)
        {
            //Validate specific short-circut states
            if (tradeLines == null || tradeLines.Count() == 0)
                return 0;
            if (annualIncome <= 0)
                throw new ArgumentException($"{nameof(annualIncome)} must have income to provide meaningful data.");

            //For all secured or unsecured trade lines which are not a mortgage, get the sum of the monthly payments.
            var monthlyDebt = tradeLines
                .Where(x => x.isMortgage == false &&
                    (x.type == UNSECURED_TRADE_LINE_TYPE || x.type == SECURED_TRADE_LINE_TYPE))
                .Sum(x => x.monthlyPayment);
            //We round this down to zero to avoid over estimating a user's monthly income.
            decimal monthlyIncome = Math.Round((annualIncome / 12m), 2, MidpointRounding.ToZero);

            return (monthlyDebt / monthlyIncome);
        }

        /// <summary>
        /// Gets the sum of any unsecured tradeline balances
        /// </summary>
        /// <returns>0 if no tradelines, otherwise the sum of unsecured balances</returns>
        private int GetUnsecuredDebtBalance(IEnumerable<TradeLine> tradeLines)
        {
            if (tradeLines == null || tradeLines.Count() == 0)
                return 0;

            //For all unsecured tradelines with a debt balance, add them together
            return tradeLines.Where(x => x.type == UNSECURED_TRADE_LINE_TYPE).Sum(x => x.balance);
        }
    }
}
