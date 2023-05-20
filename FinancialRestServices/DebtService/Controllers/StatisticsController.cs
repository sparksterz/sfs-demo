using DebtService.Business.Services;
using DebtService.Data.Entities;
using DebtService.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DebtService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly ILogger<StatisticsController> _logger;
        private readonly ICreditReportService _creditReportService;
        private readonly IStatisticCalculationService _statisticCalculationService;

        public StatisticsController(ILogger<StatisticsController> logger, ICreditReportService creditReportService, 
            IStatisticCalculationService statisticsCalculationService)
        {
            _logger = logger;
            _creditReportService = creditReportService;
            _statisticCalculationService = statisticsCalculationService;
        }

        /// <summary>
        /// Calculates debt statistics based off the given credit report application's data while taking into account
        /// annual income.
        /// </summary>
        /// <param name="applicationId">The credit report to retrieve</param>
        /// <param name="annualIncome">The yearly income to take into account when caluclating</param>
        /// <returns>Debt calculation if report was found, 500 if error accessing report, 500 if there's an error in calculations.</returns>
        [HttpGet]
        [Route("Debts")]
        public async Task<IActionResult> GetDebts([FromQuery]int applicationId, [FromQuery]int annualIncome)
        {
            _logger.LogInformation($"{nameof(StatisticsController)}.{nameof(GetDebts)} Parameters:" +
                $"{nameof(applicationId)}={applicationId}, {nameof(annualIncome)}={annualIncome}");

            //Fetch the credit report data
            IEnumerable<TradeLine> tradelines;
            try
            {
                tradelines = await _creditReportService.GetCreditReportTradeLines(applicationId);
            }
            catch(Exception reportEx)
            {
                _logger.LogError($"{nameof(StatisticsController)}.{nameof(GetDebts)} " +
                    $"- Unable to fetch credit report for application id: {applicationId}", reportEx);
                return StatusCode(500, "Unable to retrieve credit report");
            }
 
            //Calculate the debt statistics and return the result
            try
            {
                return Ok(_statisticCalculationService.CalculateDebtStatistics(tradelines, annualIncome));
            }
            catch(Exception calcEx)
            {
                _logger.LogError($"{nameof(StatisticsController)}.{nameof(GetDebts)} " +
                    $"- Couldn't calculate debt statistics for application: {applicationId} with annual income: {annualIncome}", calcEx);
                return StatusCode(500, "Error calculating Debt Statistics");
            }
        }
    }
}