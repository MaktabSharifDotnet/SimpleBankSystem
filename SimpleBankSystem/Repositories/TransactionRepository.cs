using SimpleBankSystem.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleBankSystem.Entities;


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

        
    }
}
