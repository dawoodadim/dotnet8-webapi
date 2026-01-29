using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MiddlewareFilterDI.Data;
using Newtonsoft.Json;

namespace MiddlewareFilterDI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViewDataController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _db;

        public ViewDataController(IConfiguration configuration, MyDbContext db)
        {
            _configuration = configuration;
            _db = db;
        }

        [HttpGet("ViewData")]
        public async Task<IActionResult> ViewData()
        {
            try
            {
                var details = await
                (
                    from t in _db.TicketSaleMasters
                    join p in _db.CoralPayPayments
                        on ("CPAY" + t.Pg_Transactionid.ToString())
                        equals p.systemtxnid.ToString()
                    where t.TicketStatus == "SUCCESS"
                    orderby t.TicketId descending
                    select new
                    {
                        t.TicketId,
                        t.TicketStatus,
                        t.Pg_Transactionid,
                        p.Amount
                    }
                )
                .ToListAsync();


                if (details != null)
                {
                    return Ok(new
                    {
                        Status = "0",
                        Message = "Data Found",
                        TicketDetails = details
                    });
                }

                return Ok(new
                {
                    Status = "0",
                    Message = "Ticket Not Found",
                    TicketDetails = (object?)null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "-1",
                    Message = "Internal Server Error",
                    Error = ex.Message
                });
            }
        }

    }
}
