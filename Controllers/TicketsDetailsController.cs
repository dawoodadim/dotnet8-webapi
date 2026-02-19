using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MiddlewareFilterDI.Data;
using MiddlewareFilterDI.Models;
using Newtonsoft.Json;
using MiddlewareFilterDI.Hubs;
using static MiddlewareFilterDI.Controllers.LoginController;
using Microsoft.Data.SqlClient;
using System.Data;
using Newtonsoft.Json.Linq;

namespace MiddlewareFilterDI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsDetailsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _db;
        private readonly ILogger<TicketsDetailsController> _logger;
        private readonly IHubContext<NotificationHub>? _hubContext;


        public TicketsDetailsController(IConfiguration configuration, MyDbContext db, ILogger<TicketsDetailsController> logger)
        {
            _configuration = configuration;
            _db = db;
            _logger = logger;
        }
        [Authorize]
        [HttpGet("TicketDetails")]
        public async Task<IActionResult> Detials(string Email)
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

        //[Authorize]
        [HttpPost("TicketApproval")]
        public async Task<IActionResult> TicketApproval(string TicketId)
        {
            var con = (SqlConnection)_db.Database.GetDbConnection();
            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constring"].ToString());
            
            string response = "";
            try
            {
                string checkQuery = "SELECT TicketUsed FROM [tbl_TicketSaleMaster] WHERE TicketId = '" + TicketId + "'";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                {
                    checkCmd.CommandType = CommandType.Text;
                    //checkCmd.Parameters.AddWithValue("@TicketId", TicketId);
                    con.Open();

                    object ticketUsedValue = checkCmd.ExecuteScalar(); // Get TicketUsed value
                    con.Close();

                    if (ticketUsedValue != null && ticketUsedValue.ToString() == "1")
                    {
                        // If TicketUsed is already 1, return a response without updating
                        //response = "{\n \"Status\":\"1\",\n \"UserDetails\":" + JsonConvert.SerializeObject("Ticket already used") + " \n}";

                        response = JsonConvert.SerializeObject(new
                        {
                            Status = "1",
                            UserDetails = "Ticket already used"
                        });
                    }
                    else
                    {

                        string qry = "UPDATE [tbl_TicketSaleMaster] SET TicketUsed = '1', TicketUsedOn = GETDATE() WHERE  TicketId = '" + TicketId + "' ";
                        SqlCommand cmd = new SqlCommand(qry, con);
                        cmd.CommandType = CommandType.Text;
                        con.Open();
                        int i = cmd.ExecuteNonQuery();
                        con.Close();
                        if (i > 0)
                        {
                            //response = "{\n \"Status\":\"0\",\n \"UserDetails\":" + JsonConvert.SerializeObject("UPDATED") + " \n}";
                            response = JsonConvert.SerializeObject(new
                            {
                                Status = "0",
                                Message = "Updated"
                            });
                        }
                        else
                        {
                            //response = "{\n \"Status\":\"1\",\n \"UserDetails\":" + JsonConvert.SerializeObject("Not found") + " \n}";
                            response = JsonConvert.SerializeObject(new
                            {
                                Status = "0",
                                Message = "Ticket Not Found"
                            });
                        }
                    }
                }
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
            Response.StatusCode = StatusCodes.Status200OK;
            return Content(response, "application/json");
        }

    }
}
