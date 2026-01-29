using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiddlewareFilterDI.Data;
using MiddlewareFilterDI.Models;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize] // 🔐 JWT required
public class TicketController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly ILogger<TicketController> _logger;

    public TicketController(MyDbContext db, ILogger<TicketController> logger)
    {
        _db = db;
        _logger = logger;
    }
    private static string GenerateTicketId()
    {
        var random = new Random();
        long number = random.NextInt64(1_000_000_000L, 9_999_999_999L);
        return $"DA_{number}";
    }


    [HttpPost("create")]
    public async Task<IActionResult> CreateTicket([FromBody] TicketDetails model)
    {
        _logger.LogInformation("Ticket creation request received for Email: {Email}, Name: {Name}", model.Email, model.Name);

        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for ticket creation. Email: {Email}", model.Email);
                return BadRequest(ModelState);
            }

            model.TicketCreatedNo = DateTime.UtcNow;
            model.PG_Transactionid = $"DA_{model.phoneno}_{DateTime.UtcNow:yyyyMMddHHmmss}";
            model.TicketId = GenerateTicketId();
            _db.TicketDetails.Add(model);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Ticket created successfully. TicketId: {TicketId}, Email: {Email}", model.TicketId, model.Email);

            return Ok(new
            {
                Status = "0",
                Message = "Ticket created successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating ticket. Email: {Email}", model.Email);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Status = "-1",
                Message = "An error occurred while creating ticket",
                Error = ex.Message
            });
        }
    }

    [HttpGet("ticketscount")]
    public async Task<IActionResult> GetTicketsCount()
    {
        _logger.LogInformation("Fetching tickets count started.");

        try
        {
            // Treat tickets with null/empty/"0" amount as free, others as paid
            var totalCount = await _db.TicketDetails.CountAsync();
            var freeCount = await _db.TicketDetails
                .CountAsync(t => t.TicketAmount == null || t.TicketAmount == "" || t.TicketAmount == "0");
            var paidCount = totalCount - freeCount;

            _logger.LogInformation("Tickets count fetched successfully. Total: {Total}, Free: {Free}, Paid: {Paid}", 
                totalCount, freeCount, paidCount);

            return Ok(new
            {
                Status = "0",
                Message = "Tickets count fetched successfully",
                TotalTickets = totalCount,
                FreeTickets = freeCount,
                PaidTickets = paidCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching tickets count.");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Status = "-1",
                Message = "An error occurred while fetching tickets count",
                Error = ex.Message
            });
        }
    }
}
