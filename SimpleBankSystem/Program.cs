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
        Console.WriteLine("please enter cardNumber:");
        string cardNumber = Console.ReadLine();
        Console.WriteLine("please enter pass");
        string password = Console.ReadLine();
        try
        {
            serviceCard.Authentication(cardNumber, password);
            Console.WriteLine("Authentication is successfully");
        }
        catch (CardNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (CardInactiveException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (InvalidCardNumberLengthException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (PasswordWrongException e)
        {
            Console.WriteLine(e.Message);
        }

    }
    else
    {
        ShowMenu();
        try
        {
            int option = int.Parse(Console.ReadLine());
            switch (option)
            {
                case 1:
                    
                    Console.WriteLine("please enter destinationCard");
                    string destinationCode = Console.ReadLine();
                    Console.WriteLine("please enter amount:");
                    float amount = float.Parse(Console.ReadLine());
                    try 
                    {
                        serviceCard.Transfer(LocalStorage.LoginCard.CardNumber, destinationCode, amount);
                        Console.WriteLine("transfer is done");
                    }
                    catch (NotCardLoginException e) 
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (InvalidCardNumberLengthException e) 
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch(NegativeTransferAmountException e) 
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch(CardNotFoundException e) 
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch(CardInactiveException e) 
                    {
                        Console.WriteLine(e.Message);                    
                    }
                    catch (NotEnoughBalanceException e) 
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case 2:

                    break;
            }
        }
        catch (FormatException)
        {
            Console.WriteLine("invalid option please enter 1.Transfer or 2.Reporting ");
        }
    }

}
void ShowMenu()
{
    Console.WriteLine("Please enter the number of the desired option.");
    Console.WriteLine("1.Transfer");
    Console.WriteLine("2.Reporting");
}
