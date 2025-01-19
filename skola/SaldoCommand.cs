using System.Data;
using IMeanuService;
using Npgsql;

public class SaldoCommand : Command
{
    public SaldoCommand(IAccountService accountService, IMenuService menuService)
        : base("saldo", accountService, menuService) { }

    public override void Execute(string[] args)
    {
        try
        {
            accountService.VisaSaldo();
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
