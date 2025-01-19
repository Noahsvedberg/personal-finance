public abstract class Command
{
    public string name { get; init; }
    protected IAccountService accountService;
    protected IMenuService menuService;

    public Command(string name, IAccountService accountService, IMenuService menuService)
    {
        this.name = name;
        this.accountService = accountService;
        this.menuService = menuService;
    }

    public abstract void Execute(string[] args);
}
