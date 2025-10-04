using SimpleBankSystem.DataAccess;
using SimpleBankSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBankSystem.Repositories
{
    
    public class CardRepository
    {
        private readonly AppDbContext _context;
        
        public CardRepository(AppDbContext context) 
        {
           _context = context;
        }

        public void SaveChange() 
        {
           _context.SaveChanges();
        }
        public Card? GetCard(string cardNumber) 
        {
           return _context.Cards.FirstOrDefault(c=>c.CardNumber==cardNumber);
        }

        public void UpdateCard(Card card) 
        {
            _context.Cards.Update(card);
        }


    }
}
