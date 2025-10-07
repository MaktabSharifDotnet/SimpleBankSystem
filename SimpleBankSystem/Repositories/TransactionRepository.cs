using SimpleBankSystem.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleBankSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace SimpleBankSystem.Repositories
{
    public class TransactionRepository
    {
        private readonly AppDbContext _context;
        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
        }

        public List<Transaction> GetTransactions(string cardNumber)
        {
            return _context.Transactions
                  .AsNoTracking()
                  .Where(t => t.DestinationCardNumber == cardNumber || t.SourceCardNumber == cardNumber)
                  .OrderByDescending(t => t.TransactionDate)
                  .ToList();
        }

        public float GetTotalSentAmountToday(int cardId)
        {
           
            var today = DateTime.Today;            
            var tomorrow = today.AddDays(1);

            return _context.Transactions
                .AsNoTracking()
                .Where(t => t.TransactionDate >= today 
                && t.TransactionDate < tomorrow 
                && t.SourceCardId==cardId
                &&t.IsSuccessful)
                .Sum(t => t.Amount);
        }
    }
}