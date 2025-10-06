using SimpleBankSystem.Entities;
using SimpleBankSystem.Exceptions;
using SimpleBankSystem.Manager;
using SimpleBankSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SimpleBankSystem.Exceptions;

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
        public void Authentication(string cardNumber, string password)
        {
            Card? card=_cardRepository.GetCard(cardNumber);
            if (card == null) 
            {
                throw new CardNotFoundException("There is no card with this card number.");
            }
        
            if (!card.IsActive)
            {
                throw new CardInactiveException("The card is blocked.");
            }
            if (cardNumber.Length<16) 
            {
                throw new InvalidCardNumberLengthException("The card number must be 16 digits long.");
            }

            if (card.Password != password)
            {
                card.FailedAttemptCount++;

                if (card.FailedAttemptCount >= 3)
                {
                    card.IsActive = false; 
                }             
                _cardRepository.SaveChange();              
                if (!card.IsActive)
                {
                    throw new CardInactiveException("Your card has been blocked due to 3 failed password attempts.");
                }
                else
                {
                    throw new PasswordWrongException($"The password entered is incorrect. Attempt {card.FailedAttemptCount} of 3.");
                }
            }

            card.FailedAttemptCount = 0;
            _cardRepository.SaveChange();
            LocalStorage.LoginCard = card;
        }
        public void Transfer(string sourceCard , string destinationCard , float transferAmount) 
        {
            Card? sourceCardDb = _cardRepository.GetCard(sourceCard);
            Card? destinationCardDb =_cardRepository.GetCard(destinationCard); 

            if (LocalStorage.LoginCard==null)
            {
                throw new NotCardLoginException("No card has been logged in.");
            }
            if (LocalStorage.LoginCard.CardNumber!= sourceCard)
            {
                throw new UnauthorizedAccessException("The original card number is not the same as the logged-in card number.");
            }
            if (sourceCard.Length<16) 
            {
                throw new InvalidCardNumberLengthException("The source card number is less than 16 digits.");
            }
            if (destinationCard.Length<16)
            {
                throw new InvalidCardNumberLengthException("The destinationCard number is less than 16 digits.");
            }
            if (transferAmount<0)
            {
                throw new NegativeTransferAmountException("The transfer amount must not be zero or negative.");
            }
            if (sourceCardDb==null)
            {
                throw new CardNotFoundException("invalid sourceCard ");
            }
            if (destinationCardDb == null)
            {
                throw new CardNotFoundException("invalid destinationCard ");
            }
            if (!sourceCardDb.IsActive)
            {
                throw new CardInactiveException("sourceCard is inActive");
            }
            if (!destinationCardDb.IsActive) 
            {
                throw new CardInactiveException("destinationCard is inActive");
            }
            if (sourceCardDb.Balance<transferAmount)
            {
                throw new NotEnoughBalanceException("Insufficient card balance");
            }

            sourceCardDb.Balance = sourceCardDb.Balance-transferAmount;
            destinationCardDb.Balance=destinationCardDb.Balance+transferAmount;
            Transaction transaction = new Transaction
            {
                TransactionDate = DateTime.Now,
                IsSuccessful = true,
                Amount = transferAmount,
                SourceCardId = sourceCardDb.Id,
                DestinationCardId = destinationCardDb.Id,
                SourceCardNumber = sourceCardDb.CardNumber,
                DestinationCardNumber = destinationCardDb.CardNumber
            };
            _transactionRepository.AddTransaction(transaction);
            _cardRepository.SaveChange();
        }

        public List<Transaction> GetTransactions() 
        {
            if (LocalStorage.LoginCard == null)
            {
                throw new NotCardLoginException("No card has been logged in.");
            }
           return _transactionRepository.GetTransactions(LocalStorage.LoginCard.CardNumber);
        }
    }
}