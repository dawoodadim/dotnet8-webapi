using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiddlewareFilterDI.Data;
using Newtonsoft.Json;

namespace MiddlewareFilterDI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class TableIdentifyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _db;

        public TableIdentifyController(IConfiguration configuration, MyDbContext db)
        {
            _configuration = configuration;
            _db = db;
        }
        [HttpGet("TableIdentify")]

        public async Task<IActionResult>tableidentify(string Pg_TransactionId)
        {
            string jsonResp = "";
            try
            {
                string cpayId = "CPAY" + Pg_TransactionId;
                string opayId = "OPAY" + Pg_TransactionId;

                if (await _db.CoralPayPayments.AnyAsync(c => c.systemtxnid == cpayId))
                    return Ok(new {PaymentType = "CPAY" });

                if (await _db.OPayPayment.AnyAsync(o => o.systemtxnid == opayId))
                    return Ok(new {PaymentType = "OPAY" });

                return Ok(new {PaymentType = "NOT FOUND" });
            }
            catch (Exception ex)
            {
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
