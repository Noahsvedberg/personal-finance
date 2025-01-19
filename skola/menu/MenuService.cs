public interface IMenuService
{
    Menu GetMenu();
    void SetMenu(Menu menu);
}

public class SimpleMenuService : IMenuService
{
    private Menu? currentMenu;

    public Menu GetMenu()
    {
        if (currentMenu == null)
        {
            throw new ArgumentException("No current Menu is set!");
        }
        return currentMenu;
    }

    
    public void SetMenu(Menu menu)
    {
        currentMenu = menu;
        currentMenu.Display();
    }
}

class EmptyMenu : Menu
{
    public override void Display() { }
}