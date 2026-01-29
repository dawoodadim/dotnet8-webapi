using Microsoft.AspNetCore.Mvc;
using MiddlewareFilterDI.Data;
using Microsoft.EntityFrameworkCore;

namespace MiddlewareFilterDI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NationalityController : ControllerBase
    {
        private readonly MyDbContext _db;
        private readonly ILogger<NationalityController> _logger;

        public NationalityController(MyDbContext db, ILogger<NationalityController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet("nationality-summary")]
        public async Task<IActionResult> GetNationalitySummary()
        {
            _logger.LogInformation("Fetching nationality summary started.");

            try
            {
                var result = await _db.TicketDetails
                    .Where(x => x.Nationality != null)
                    .GroupBy(x => x.Nationality)
                    .Select(g => new
                    {
                        Nationality = g.Key,
                        TicketsWithAmount = g.Count(x => x.TicketAmount != "0"),
                        TicketsWithZeroAmount = g.Count(x => x.TicketAmount == "0")
                    })
                    .ToListAsync();

                _logger.LogInformation("Nationality summary fetched successfully. Count: {Count}", result.Count);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching nationality summary.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching nationality summary.");
            }
        }
    }
}
