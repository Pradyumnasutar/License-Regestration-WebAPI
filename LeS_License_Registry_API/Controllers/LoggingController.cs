using LeS_License_Registry_API.Data;
using LeS_License_Registry_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeS_License_Registry_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoggingController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly ILogger<LoggingController> _logger;
        private readonly LesLicenseRegistryContext _dataAccess;
        private readonly IConfiguration _configuration;
        public LoggingController(TokenService tokenService, ILogger<LoggingController> logger, LesLicenseRegistryContext dataAccess, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _logger = logger;
            _dataAccess = dataAccess;
            _configuration = configuration;
        }
        [HttpGet]
        [Route("GetLogActivities")]
        public IActionResult GetLogActivities()
        {
            API_Response response = new API_Response();
            response.isSuccess = false;
            try
            {
                var DefaultPeriod = Convert.ToInt32(_configuration["Appsettings:ACTIVITY_LOG_PERIOD"]);
                DateTime threeMonthsAgo = DateTime.Now.AddMonths(-DefaultPeriod);

                var allActivities = _dataAccess.v_les_license_update_log
                    .Where(log => log.updated_date >= threeMonthsAgo)  // Filter last 3 months
                    .OrderBy(x=>x.logid).ToList();

                response.totalRecords = allActivities.Count;
                response.isSuccess = true;
                response.data = allActivities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading GetLogActivities!");
                response.isSuccess = false;
                response.message = "Internal server error!";
            }
            return Ok(response);
        }

    }
}
