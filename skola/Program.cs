using System;
using System.Collections.Generic;
using Npgsql;

class Program  
{
    public static void Main() 
    {
        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=password;Database=bank;";
        using var connection = new NpgsqlConnection(connectionString);

        connection.Open();

         var createTableSql = @"CREATE TABLE IF NOT EXISTS users (user_id UUID PRIMARY KEY,
        name TEXT UNIQUE,
        password TEXT UNIQUE );

        CREATE TABLE IF NOT EXISTS transactions (
        id SERIAL PRIMARY KEY,
        user_id UUID REFERENCES users(user_id) ON DELETE CASCADE,
        type VARCHAR(50) NOT NULL, -- 'income' eller 'expense'
        amount DECIMAL NOT NULL,
        transaction_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";

        using var cmd = new NpgsqlCommand(createTableSql, connection);
        cmd.ExecuteNonQuery();
    

        IAccountService accountService = new Account(connection);
        IMenuService menuService = new SimpleMenuService();
        

        Menu initialMenu = new LoginMenu(accountService, menuService);
        menuService.SetMenu(initialMenu);

         while(true)
          {
            Console.WriteLine("> ");
            string? inputCommand = Console.ReadLine()!;
            if (inputCommand.ToLower() != null)
             {
                menuService.GetMenu().ExecuteCommand(inputCommand);
            }
             else
             {
                break;
            }

        }
    }
}