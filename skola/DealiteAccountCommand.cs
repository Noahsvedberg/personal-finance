public class DeleteAccountCommand : Command
{
    public DeleteAccountCommand(IAccountService accountService, IMenuService menuService)
        : base("delete-account", accountService, menuService) { }

    public override void Execute(string[] args)
    {
        accountService.RemoveUser();
        Thread.Sleep(3000);
        menuService.SetMenu(new LoginMenu(accountService, menuService));
    }
}
