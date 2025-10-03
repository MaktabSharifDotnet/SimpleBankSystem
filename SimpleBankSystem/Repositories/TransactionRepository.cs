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

        
        public List<Transaction> GetTransactionsByCardId(int cardId)
        {
           
            return _context.Transactions
                .AsNoTracking()
                .Where(t => t.SourceCardId == cardId || t.DestinationCardId == cardId)
                .OrderByDescending(t => t.TransactionDate) 
                .ToList();
        }
    }
}