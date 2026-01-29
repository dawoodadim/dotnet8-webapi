using Microsoft.AspNetCore.Mvc;
using MiddlewareFilterDI.Data;
using Microsoft.EntityFrameworkCore;

namespace MiddlewareFilterDI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly MyDbContext _db;
        private readonly ILogger<CountryController> _logger;

        public CountryController(MyDbContext db, ILogger<CountryController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet("nationalities")]
        public async Task<IActionResult> GetNationalities()
        {
            _logger.LogInformation("Fetching nationalities started.");

            try
            {
                var data = await _db.CountryMasters
                              .Where(x => x.Nationality != null && x.Nationality != "")
                              .Select(x => new
                              {
                                  x.ID,
                                  x.Nationality
                              })
                              .OrderBy(x => x.Nationality)
                              .ToListAsync();

                _logger.LogInformation("Fetching nationalities completed. Count: {Count}", data.Count);

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching nationalities.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching nationalities.");
            }
        }
    }

}
