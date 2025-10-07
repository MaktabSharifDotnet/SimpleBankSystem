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
                    try
                    {
                        string destinationCard;
                        float amount = 0f;
                        while (true)
                        {
                            Console.WriteLine("please enter destinationCard (or type 'exit' to cancel):");
                            destinationCard = Console.ReadLine();

                            if (destinationCard.ToLower() == "exit")
                            {
                                break;
                            }

                            try
                            {
                                
                                ShowHolderName(destinationCard);
                                Console.WriteLine("is it correct? 1.yes 2.No");
                                int result = int.Parse(Console.ReadLine());

                                if (result == 1)
                                {
                                    
                                    break;
                                }
                              
                            }
                            catch (CardNotFoundException e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine("Invalid input. Please enter 1 for yes or 2 for No.");
                            }
                        }                      
                        if (destinationCard.ToLower() != "exit")
                        {                          
                            Console.WriteLine("please enter amount:");
                            amount = float.Parse(Console.ReadLine());                          
                            try
                            {
                                
                                serviceCard.GenerateAndSaveVerificationCode();
                                Console.WriteLine("\nA 5-digit verification code has been generated (check verification_code.txt).");
                                
                                Console.WriteLine("Please enter the verification code to complete the transfer:");
                                string userCode = Console.ReadLine();                  
                                serviceCard.VerifyCode(userCode);                               
                                serviceCard.Transfer(LocalStorage.LoginCard.CardNumber, destinationCard, amount);
                                Console.WriteLine("transfer is done");
                            }
                            catch (Exception e) 
                            {
                                Console.WriteLine($"Error: {e.Message}");
                                Console.WriteLine("Transfer cancelled.");
                            }                          
                        }
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
                    catch (DailyTransferLimitExceededException e) 
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (FormatException) 
                    {
                        Console.WriteLine("invalid option please enter number 1.yes 2.No");
                    }
                    break;
                case 2:
                    try 
                    {
                        List<Transaction> transactions=serviceCard.GetTransactions();
                        if (transactions.Count==0)
                        {
                            Console.WriteLine("No transaction has been made yet.");
                        }
                        else 
                        {
                            foreach (var transaction in transactions)
                            {

                                string type = transaction.SourceCardNumber == LocalStorage.LoginCard.CardNumber ? "Sent" : "Received";
                                string status = transaction.IsSuccessful ? "Successful" : "Failed";                             
                                Console.WriteLine($"Type: {type} | From: {transaction.SourceCardNumber} | To: {transaction.DestinationCardNumber} | Amount: {transaction.Amount} | Date: {transaction.TransactionDate.ToShortDateString()} | Status: {status}");
                            }
                        }
                    }
                    catch(NotCardLoginException e) 
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case 3:
                    Console.WriteLine("please enter new pass");
                    string newPass = Console.ReadLine();
                    try 
                    {
                        serviceCard.ChangeCardPassword(newPass);
                        Console.WriteLine("change pass is done");
                    }
                    catch (NotCardLoginException e) 
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (InvalidPassException e) 
                    {
                        Console.WriteLine(e.Message);
                    }
                   
                   break;
                case 4:
                    LocalStorage.LoginCard = null;
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
    Console.WriteLine("3.ChangePass");
    Console.WriteLine("4.Exit");
}

void ShowHolderName(string cardNumber) 
{
    string? holderName = serviceCard.GetHolderNameByCardNumber(cardNumber);
    Console.WriteLine($"Destination account holder name :{holderName}");
}
