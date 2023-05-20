using Microsoft.AspNetCore.Mvc;
using ReportService.Data.Exceptions;
using ReportService.Data.Services;

namespace ReportService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreditController : ControllerBase
    {
        private readonly ILogger<CreditController> _logger;
        private readonly ICreditDataService _creditDataService;

        public CreditController(ILogger<CreditController> logger, ICreditDataService creditDataService)
        {
            _logger = logger;
            _creditDataService = creditDataService;
        }

        /// <summary>
        /// Fetches a single credit report from the respective source and bureau given an applicationId.
        /// </summary>
        /// <param name="applicationId">Id of the credit report application</param>
        /// <param name="source">The source which originated the application</param>
        /// <param name="bureau">The credit bureau to retrieve data from</param>
        /// <returns>Will return either a single report if found, null report if not, and a 500 error in the case of multiple matching records</returns>
        [HttpGet]
        [Route("Report")]
        public IActionResult GetReport([FromQuery]int applicationId, [FromQuery]string source, [FromQuery]string bureau)
        {
            _logger.LogDebug($"{nameof(CreditController)}.{nameof(GetReport)}: Parameters:" +
                $"{nameof(applicationId)}={applicationId}, {nameof(source)}={source}, {nameof(bureau)}={bureau}");
            
            try
            {
                var response = _creditDataService.GetCreditReport(applicationId, source, bureau);
                return Ok(response);
            }
            catch(UnexpectedMultipleItemException unex)
            {
                _logger.LogError($"{nameof(CreditController)}.{nameof(GetReport)}: Exception calling {nameof(ICreditDataService)}.{nameof(ICreditDataService.GetCreditReport)}", unex);
                return StatusCode(500, "Multiple credit reports found with given criteria.");
            }
        }
    }
}