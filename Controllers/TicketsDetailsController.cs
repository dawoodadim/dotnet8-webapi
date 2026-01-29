using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiddlewareFilterDI.Data;
using Newtonsoft.Json;
using static MiddlewareFilterDI.Controllers.LoginController;

namespace MiddlewareFilterDI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsDetailsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _db;
        private readonly ILogger<TicketsDetailsController> _logger;

        public TicketsDetailsController(IConfiguration configuration, MyDbContext db, ILogger<TicketsDetailsController> logger)
        {
            _configuration = configuration;
            _db = db;
            _logger = logger;
        }
        [Authorize]
        [HttpGet("TicketDetails")]
        public async Task<IActionResult> login(string Email)
        {
            _logger.LogInformation("Fetching ticket details for Email/Name/TicketId: {Email}", Email);

            string jsonResp = "";
            try
            {
                var details = await _db.TicketDetails
                    .Where (u => u.Email == Email || u.Name == Email || u.TicketId == Email)
                    .Select(t=> new
                    {
                        t.TicketId,
                        t.Name,
                        t.PG_Transactionid,
                        t.EventName,
                        t.TicketCreatedNo,
                    })
                    .ToListAsync();

                if (details != null && details.Any())
                {
                    _logger.LogInformation("Ticket details found for Email/Name/TicketId: {Email}. Count: {Count}", Email, details.Count);

                    jsonResp = JsonConvert.SerializeObject(new
                    {
                        Status = "0",
                        Message = "Data Found",
                        ticketdetails= details
                    });
                }
                else
                {
                    _logger.LogWarning("No ticket details found for Email/Name/TicketId: {Email}", Email);
                    jsonResp = JsonConvert.SerializeObject(new
                    {
                        Status = "0",
                        Message = "Ticket Not Found",
                        ticketdetails = Array.Empty<object>()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching ticket details for Email/Name/TicketId: {Email}", Email);
                jsonResp = JsonConvert.SerializeObject(new
                {
                    Status = "-1",
                    Message = Array.Empty<object>(),
                    Error = ex.Message
                });
            }
            Response.StatusCode = StatusCodes.Status200OK;
            return Content(jsonResp, "application/json");
        }
    }
}
