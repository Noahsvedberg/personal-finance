public abstract class Menu
{
    private List<Command> commands = new List<Command>();

    public void AddCommand(Command command)
    {
        this.commands.Add(command);
    }

    public void ExecuteCommand(string inputCommand)
    {
        if (string.IsNullOrWhiteSpace(inputCommand))
        {
            throw new ArgumentException("Command cannot be emply");
        }

        string[] commandParts = inputCommand.Split(" ");
        string commandName = commandParts[0].ToLower();

        Command? matchningCommand = commands.FirstOrDefault(cmd =>
            cmd.name.Equals(commandName, StringComparison.OrdinalIgnoreCase)
        );

        if (matchningCommand != null)
        {
            matchningCommand.Execute(commandParts);
            return;
        }
        throw new ArgumentException(
            $"command '{commandName}' not found. Type 'help' to see available commands!"
        );

        throw new ArgumentException("Command not found.");
    }

    public abstract void Display();
}
