using Microsoft.EntityFrameworkCore;
using SimpleBankSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBankSystem.DataAccess
{
    public class AppDbContext :DbContext
    {
        public DbSet<Card> Cards { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=ALI\\SQLEXPRESS;Database=SimpleBankSystem;Integrated Security=True;Encrypt=Mandatory;TrustServerCertificate=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.SourceCard)
                .WithMany(c => c.SentTransactions)
                .HasForeignKey(t => t.SourceCardId)
                .OnDelete(DeleteBehavior.NoAction); 

          
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.DestinationCard)
                .WithMany(c => c.ReceivedTransactions)
                .HasForeignKey(t => t.DestinationCardId)
                .OnDelete(DeleteBehavior.NoAction); 
        }

    }
}
