namespace MiddlewareFilterDI.Models
{
    public class TicketSaleMaster
    {
        public string? TicketId { get; set; }
        public string? TicketStatus { get; set; }
        public string? Pg_Transactionid { get; set; }
        public DateTime? TicketCreatedNo { get; set; }
    }

    public class CoralPayPayment
    {
        public string systemtxnid { get; set; }
        public Decimal Amount { get; set; }
    }

}
