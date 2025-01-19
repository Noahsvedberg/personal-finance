using IMeanuService;

public class LoginMenu : Menu
{

public LoginMenu(IAccountService accountService, IMenuService menuService)
{
        AddCommand(new LoginCommand(accountService, menuService));
        AddCommand(new RegisterCommand(accountService, menuService));
}

    public override void Display()
    {
        Console.Clear();
        Console.WriteLine($"               \nWELCOME TO YOUR BANK APP  ");
        Console.WriteLine($"||login <username> <password> - Log into your account");
        Console.WriteLine($"||register-user <username> <password> - Create a new account");
    }
}