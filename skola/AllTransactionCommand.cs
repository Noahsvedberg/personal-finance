public class AllTransactionCommand : Command
{
    public AllTransactionCommand(IAccountService accountService, IMenuService menuService)
        : base("all-transactions", accountService, menuService) { }

    public override void Execute(string[] args)
    {
        try
        {
            accountService.AllaTransaktioner();
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