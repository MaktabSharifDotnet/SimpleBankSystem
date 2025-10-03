using SimpleBankSystem.Entities;
using SimpleBankSystem.Exceptions;
using SimpleBankSystem.Manager;
using SimpleBankSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBankSystem.Services
{
    public class ServiceCard
    {
        private readonly CardRepository _cardRepository;
        private readonly TransactionRepository _transactionRepository;
        public ServiceCard(CardRepository cardRepository , TransactionRepository transactionRepository)
        {
            _cardRepository = cardRepository;
            _transactionRepository = transactionRepository;
        }

        public void Transfer(string sourceCardNumber, string destinationCardNumber, float depositAmount)
        {
            Card? sourceCard = _cardRepository.GetCardByCardNumber(sourceCardNumber);
            Card? destinationCard = _cardRepository.GetCardByCardNumber(destinationCardNumber);
            if (sourceCard == null ) 
            {
                throw new CardNotFoundException("The sourceCard is not valid..");
            }
            if (destinationCard == null)
            {
                throw new CardNotFoundException("The destinationCard is not valid..");
            }
            if (sourceCard.Balance < depositAmount) 
            {
                throw new NotEnoughBalanceException("Insufficient card balance");
            }
            if (!sourceCard.IsActive) 
            {
                throw new CardInactiveException("The origin card is inactive.");
            }
            if (!destinationCard.IsActive) 
            {
                throw new CardInactiveException("The destinationCard is inactive.");
            }
            if (depositAmount<0) 
            {
                throw new NegativeTransferAmountException("The transfer amount must not be negative.");
            }
            if (sourceCardNumber.Length<16)
            {
                throw new InvalidCardNumberLengthException("The sourceCard number is less than 16 digits.");
            }
            if (destinationCardNumber.Length<16) 
            {
                throw new InvalidCardNumberLengthException("The destinationCard number is less than 16 digits.");
            }
            sourceCard.Balance  = sourceCard.Balance - depositAmount;
            destinationCard.Balance = destinationCard.Balance + depositAmount;
            Transaction transaction = new Transaction 
            {
               Amount = depositAmount,
               SourceCardId = sourceCard.Id,
               DestinationCardId = destinationCard.Id,
               TransactionDate = DateTime.Now,
               IsSuccessful = true,
            };
            _transactionRepository.AddTransaction(transaction);
            _cardRepository.UpdateCard(sourceCard);
            _cardRepository.UpdateCard(destinationCard);
            _cardRepository.SaveChanges();
        }
        public void Authentication(string cardNumber , string password) 
        {
            Card? card= _cardRepository.GetCardByCardNumber(cardNumber);
            if (card == null) 
            {
                throw new CardNotFoundException("No card with this card number was found.");
            }
            if (card.Password!=password)
            {
                throw new PasswordWrongException("The card password is incorrect.");
            }
            if (cardNumber.Length < 16)
            {
                throw new InvalidCardNumberLengthException("The cardNumber is less than 16 digits.");
            }
            LocalStorage.LoginCard= card;
        }
    }
}
