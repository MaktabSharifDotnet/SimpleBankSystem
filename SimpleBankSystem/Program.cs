using SimpleBankSystem.DataAccess;
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

           
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Login was successful! Welcome {LocalStorage.LoginCard.HolderName}.");
            Console.ResetColor();
        }
      
        catch (CardNotFoundException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }
        catch (PasswordWrongException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }
        catch (InvalidCardNumberLengthException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }
        catch (Exception ex) 
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            Console.ResetColor();
        }
    }
    else 
    {
        Console.WriteLine("\n--- Main Menu ---");
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
                      
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Transfer was successful!");
                        Console.ResetColor();
                    }
                  
                    catch (FormatException)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Invalid amount. Please enter a valid number.");
                        Console.ResetColor();
                    }
                    catch (CardNotFoundException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                    }
                    catch (NotEnoughBalanceException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                    }
                    catch (CardInactiveException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                    }
                    catch (NegativeTransferAmountException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                    }
                    catch (InvalidCardNumberLengthException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                    }
                    catch (Exception ex) 
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                        Console.ResetColor();
                    }
                   
                    break;

                case 2:
                    // این بخش هنوز پیاده‌سازی نشده است
                    Console.WriteLine("Show Transactions functionality is not implemented yet.");
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Invalid option. Please enter 1 or 2.");
                    Console.ResetColor();
                    break;
            }
        }
        catch (FormatException)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Invalid input. Please select a valid menu option (1 or 2).");
            Console.ResetColor();
        }
    }
}