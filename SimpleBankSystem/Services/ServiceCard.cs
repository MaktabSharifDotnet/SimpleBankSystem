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
            

            float totalAmount=_transactionRepository.GetTotalSentAmountToday(LocalStorage.LoginCard.Id);
            if (totalAmount+ transferAmount > 250) 
            {
                throw new DailyTransferLimitExceededException("You have reached the daily transfer limit ($250).");
            }

            float fee = 0;
            if (transferAmount>1000)
            {
                fee = 0.015f * transferAmount;
            }
            else if (transferAmount <= 1000)
            {
                fee = 0.005f * transferAmount;
            }
            float totalDeduction = transferAmount + fee;
            if (sourceCardDb.Balance < totalDeduction)
            {
                throw new NotEnoughBalanceException("Insufficient card balance");
            }
            sourceCardDb.Balance -= totalDeduction; 
            destinationCardDb.Balance += transferAmount;
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

        public void ChangeCardPassword(string newPass) 
        {
            if (LocalStorage.LoginCard==null)
            {
                throw new NotCardLoginException("No card has been logged in.");
            }
            bool verifyPass =LocalStorage.LoginCard.SetPass(newPass);
            if (verifyPass)
            {
                _cardRepository.SaveChange();
            }
            else 
            {
                throw new InvalidPassException("The password must be 4 digits and not repeated.");
            }
            
        }

        public string? GetHolderNameByCardNumber(string cardNumber) 
        {
            if (LocalStorage.LoginCard == null)
            {
                throw new NotCardLoginException("No card has been logged in.");
            }
           string holderName = _cardRepository.GetHolderNameByCardNumber(cardNumber);
            if (holderName==null)
            {
                throw new CardNotFoundException("No card with this card number was found.");
            }
            return holderName;
        }
        public void GenerateAndSaveVerificationCode()
        {
           
            Random random = new Random();

            int code = random.Next(10000, 100000);
            
            DateTime creationTime = DateTime.Now;
          
            string contentToSave = $"{code};{creationTime}";
                       
            File.WriteAllText("verification_code.txt", contentToSave);
        }
        public void VerifyCode(string userInput)
        {
            
            if (!File.Exists("verification_code.txt"))
            {
                throw new Exception("Verification code was not generated.");
            }
            string fileContent = File.ReadAllText("verification_code.txt");
         
            string[] parts = fileContent.Split(';');
            string savedCode = parts[0];
            DateTime creationTime = DateTime.Parse(parts[1]); 
           
            if (DateTime.Now > creationTime.AddMinutes(5))
            {
                throw new Exception("Verification code has expired.");
            }
          
            if (savedCode != userInput)
            {
                throw new Exception("Verification code is incorrect.");
            }
           
        }

    }
}