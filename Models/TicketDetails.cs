using System.ComponentModel.DataAnnotations;

namespace MiddlewareFilterDI.Models
{
    public class TicketDetails
    {
        public int Id { get; set; }
        public string? TicketId { get; set; }
        public string? TicketAmount { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string phoneno { get; set; }
        public string? EventName { get; set; }
        public string? PG_Transactionid { get; set; }
        public string? Nationality { get; set; }
        public DateTime? TicketCreatedNo { get; set; }
    }
}
