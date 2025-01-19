using System.Data;
using IMeanuService;
using Npgsql;

public class UtgifterCommand : Command
{
    public UtgifterCommand(IAccountService accountService, IMenuService menuService)
        : base("expense", accountService, menuService) { }

    public override void Execute(string[] args)
    {
        try
        {
            accountService.VisaUtgifter();
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
