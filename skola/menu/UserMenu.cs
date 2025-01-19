using IMeanuService;
using Npgsql;

public class UserMenu : Menu
{

    public UserMenu(IAccountService accountService, IMenuService menuService) 
    {

        
        // AddCommand(new HelpCommand(accountService, menuService));
        AddCommand(new SaldoCommand(accountService, menuService));
        AddCommand(new AddTransactionCommand(accountService, menuService));
        AddCommand(new UtgifterCommand(accountService, menuService));
        AddCommand(new InkomsterCommand(accountService, menuService));
        AddCommand(new AllTransactionCommand(accountService, menuService));
        AddCommand(new LogoutCommand(accountService, menuService));
        AddCommand(new DeleteAccountCommand( accountService, menuService));
    }

    public override void Display()
    {
        Console.Clear();
        Console.WriteLine($"  \nDin privatekonomi:");
        Console.WriteLine();
        Console.WriteLine($"[Add-transaction] Add transaction");
        Console.WriteLine($"[Saldo] Se nuvarande kontobalans");
        Console.WriteLine($"[Expense] Se utgifter");
        Console.WriteLine($"[Income] Se inkomst");
        Console.WriteLine($"[All-transactions] Transaktioner och redara transaktioner");
        Console.WriteLine($"[Delete-account] Radera konto");
        Console.WriteLine($"[exit] Logout");


    }
    
}