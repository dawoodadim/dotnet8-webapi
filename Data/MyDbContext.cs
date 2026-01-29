using Microsoft.EntityFrameworkCore;
using MiddlewareFilterDI.Models;

namespace MiddlewareFilterDI.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
        {

        }
        public DbSet<LoginCredential> LoginCredential { get; set; }
        public DbSet<CountryMaster> CountryMasters { get; set; }
        public DbSet<TicketDetails> TicketDetails { get; set; }
        public DbSet<TicketSaleMaster> TicketSaleMasters { get; set; }
        public DbSet<CoralPayPayment> CoralPayPayments { get; set; }
        public DbSet<OPayPayment> OPayPayment {  get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginCredential>().ToTable("tbl_UserMaster").HasNoKey();
            modelBuilder.Entity<TicketDetails>().ToTable("tbl_TicketSaleMaster1").HasKey(a=> a.Id);
            modelBuilder.Entity<CountryMaster>().ToTable("tbl_CountryMaster").HasKey(a => a.ID);
            modelBuilder.Entity<TicketSaleMaster>().ToTable("tbl_TicketSaleMaster").HasNoKey();
            modelBuilder.Entity<CoralPayPayment>().ToTable("tblCoralPayPayments").HasNoKey();
            modelBuilder.Entity<OPayPayment>().ToTable("tblOPAYPaymentDetails").HasNoKey();
        }
    }
}
