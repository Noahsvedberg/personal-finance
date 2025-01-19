using System.ComponentModel.Design.Serialization;

namespace IMeanuService;
public class LoginCommand : Command
{
    public LoginCommand(IAccountService accountService, IMenuService menuService)
        : base("Login", accountService, menuService)
        {

        }

    public override void Execute(string[] args)
    {
        string username = args[1];
        string password = args[2];

        User? user = accountService.Login(username, password);
        if (user == null)
        {
            Console.WriteLine("Wrong username or password.");
            return;
        }

        Console.WriteLine("You successfully logged in.");
        menuService.SetMenu(new UserMenu(accountService, menuService));
    }
}

public class LogoutCommand : Command
{
    public LogoutCommand(IAccountService accountService, IMenuService menuService)
        : base("exit", accountService, menuService) { }

    public override void Execute(string[] args)
    {
        accountService.Logout();
        menuService.SetMenu(new LoginMenu(accountService, menuService));
    }
}
