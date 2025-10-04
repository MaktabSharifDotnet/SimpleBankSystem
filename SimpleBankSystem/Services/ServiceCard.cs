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

   
        private static readonly Dictionary<string, int> _failedLoginAttempts = new Dictionary<string, int>();

        public ServiceCard(CardRepository cardRepository, TransactionRepository transactionRepository)
        {
            _cardRepository = cardRepository;
            _transactionRepository = transactionRepository;
        }
        public void Transfer(string sourceCardNumber, string destinationCardNumber, float depositAmount)
        {
           
            if (sourceCardNumber.Length != 16)
            {
                throw new InvalidCardNumberLengthException("The sourceCard number must be 16 digits.");
            }
            if (destinationCardNumber.Length != 16)
            {
                throw new InvalidCardNumberLengthException("The destinationCard number must be 16 digits.");
            }
            if (depositAmount <= 0)
            {
                throw new NegativeTransferAmountException("The transfer amount must be greater than zero.");
            }

            Card? sourceCard = _cardRepository.GetCardByCardNumber(sourceCardNumber);
            Card? destinationCard = _cardRepository.GetCardByCardNumber(destinationCardNumber);

            if (sourceCard == null)
            {
                throw new CardNotFoundException("The sourceCard is not valid.");
            }
            if (destinationCard == null)
            {
                throw new CardNotFoundException("The destinationCard is not valid.");
            }
            if (!sourceCard.IsActive)
            {
                throw new CardInactiveException("The origin card is inactive.");
            }
            if (!destinationCard.IsActive)
            {
                throw new CardInactiveException("The destinationCard is inactive.");
            }
            if (sourceCard.Balance < depositAmount)
            {
                throw new NotEnoughBalanceException("Insufficient card balance");
            }

            sourceCard.Balance -= depositAmount;
            destinationCard.Balance += depositAmount;

            Transaction transaction = new Transaction
            {
                Amount = depositAmount,
                SourceCardId = sourceCard.Id,
                DestinationCardId = destinationCard.Id,
                TransactionDate = DateTime.Now,
                IsSuccessful = true,
                SourceCardNumber = sourceCard.CardNumber,
                DestinationCardNumber = destinationCard.CardNumber
            };

            _transactionRepository.AddTransaction(transaction);
            _cardRepository.UpdateCard(sourceCard);
            _cardRepository.UpdateCard(destinationCard);
            _cardRepository.SaveChanges();
        }
        public void Authentication(string cardNumber, string password)
        {
            if (cardNumber.Length != 16)
            {
                throw new InvalidCardNumberLengthException("The cardNumber must be 16 digits.");
            }

            Card? card = _cardRepository.GetCardByCardNumber(cardNumber);

            if (card == null)
            {
                throw new CardNotFoundException("Your card number or password is correct.");
            }

            if (!card.IsActive)
            {
                throw new CardInactiveException("This card has been blocked due to multiple failed login attempts.");
            }

            if (card.Password != password)
            {
             
                if (!_failedLoginAttempts.ContainsKey(cardNumber))
                {
                    _failedLoginAttempts[cardNumber] = 0;
                }

                _failedLoginAttempts[cardNumber]++; 

                if (_failedLoginAttempts[cardNumber] >= 3)
                {
                    card.IsActive = false; 
                    _cardRepository.UpdateCard(card);
                    _cardRepository.SaveChanges();
                    throw new CardInactiveException("Your card has been blocked due to 3 failed password attempts.");
                }

                throw new PasswordWrongException($"Your card number or password is correct. Attempt {_failedLoginAttempts[cardNumber]} of 3.");
            }

            
            if (_failedLoginAttempts.ContainsKey(cardNumber))
            {
                _failedLoginAttempts.Remove(cardNumber);
            }

            LocalStorage.LoginCard = card;
        }
        public List<Transaction> GetTransactionsForCard(int cardId)
        {
            return _transactionRepository.GetTransactionsByCardId(cardId);
        }
    }
}