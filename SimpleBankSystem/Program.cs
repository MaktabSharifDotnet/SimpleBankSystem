using SimpleBankSystem.DataAccess;
using SimpleBankSystem.Entities;
using SimpleBankSystem.Exceptions;
using SimpleBankSystem.Manager;
using SimpleBankSystem.Repositories;
using SimpleBankSystem.Services;


AppDbContext appDbContext = new AppDbContext();
CardRepository cardRepository = new CardRepository(appDbContext);
TransactionRepository transactionRepository = new TransactionRepository(appDbContext);
ServiceCard serviceCard = new ServiceCard(cardRepository, transactionRepository);

while (true)
{
    if (LocalStorage.LoginCard == null)
    {
        Console.WriteLine("\n--- Welcome to Simple Bank System ---");
        Console.WriteLine("Please enter your Card Number:");
        string cardNumber = Console.ReadLine();

        Console.WriteLine("Please enter your password:");
        string password = Console.ReadLine();

        try
        {
            serviceCard.Authentication(cardNumber, password);
            Console.WriteLine($"Login was successful! Welcome {LocalStorage.LoginCard.HolderName}.");
        }
        catch (CardInactiveException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (InvalidCardNumberLengthException ex)
        {
            Console.WriteLine(ex.Message);
        }
       
    }
    else
    {
        Console.WriteLine("--- Main Menu ---");
        Console.WriteLine("1. Transfer Money");
        Console.WriteLine("2. Show Transactions");
        Console.WriteLine("Please select an option:");

        try
        {
            int option = int.Parse(Console.ReadLine());

            switch (option)
            {
                case 1:
                    try
                    {
                        Console.WriteLine("Enter destination card number:");
                        string destinationCardNumber = Console.ReadLine();

                        Console.WriteLine("Enter amount to transfer:");
                        float amount = float.Parse(Console.ReadLine());

                        string sourceCardNumber = LocalStorage.LoginCard.CardNumber;

                        serviceCard.Transfer(sourceCardNumber, destinationCardNumber, amount);

                        Console.WriteLine("Transfer was successful!");
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid amount. Please enter a valid number.");
                    }
                    catch (VallidationException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                    }
                    break;
                case 2:
                    Console.WriteLine("\n--- Your Transactions ---");
                    int cardId = LocalStorage.LoginCard.Id;
                    var transactions = serviceCard.GetTransactionsForCard(cardId);

                    if (transactions.Count == 0)
                    {
                        Console.WriteLine("You have no transactions yet.");
                    }
                    else
                    {
                        foreach (var tx in transactions)
                        {

                            string type;
                            if (tx.SourceCardId == cardId)
                            {
                                type = "Sent";
                            }
                            else
                            {
                                type = "Received";
                            }
                            string status;
                            if (tx.IsSuccessful)
                            {
                                status = "Successful";
                            }
                            else
                            {
                                status = "Failed";
                            }
                            
                             Console.WriteLine($"Type: {type} | From: {tx.SourceCardNumber} | To: {tx.DestinationCardNumber} | Amount: {tx.Amount} " +
                                $"| Date: {tx.TransactionDate.ToShortDateString()} | Status: {status}");
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Invalid option. Please enter 1 or 2.");
                    break;
            }
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid input. Please select a valid menu option (1 or 2).");
        }
    }
}