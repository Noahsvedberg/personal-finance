using System;
using System.Threading;

public class AddTransactionCommand : Command
{
    public AddTransactionCommand(IAccountService accountService, IMenuService menuService)
        : base("add-transaction", accountService, menuService) { }

    public override void Execute(string[] args)
    {
        try
        {
            accountService.LäggTillTransaktion();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ett fel inträffade: {ex.Message}");
        }

        Console.WriteLine($"\n Press any key to go back to the menu");
        System.Console.WriteLine("\n");
        Console.ReadKey();

        menuService.SetMenu(new UserMenu(accountService, menuService));
    }
}
