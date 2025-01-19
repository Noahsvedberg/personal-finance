using Npgsql;

public class Account : IAccountService
{
    private NpgsqlConnection connection;

    private Guid? loggedInUser = null;

    public Account(NpgsqlConnection connection)
    {
        this.connection = connection;
    }

    public User RegisterUser(string username, string password)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = username,
            Password = password,
        };

        var sql =
            @"INSERT INTO users (user_id, name, password) VALUES (
            @id,
            @name,
            @password
        )";
        using var cmd = new NpgsqlCommand(sql, this.connection);
        cmd.Parameters.AddWithValue("@id", user.Id);
        cmd.Parameters.AddWithValue("@name", user.Name);
        cmd.Parameters.AddWithValue("@password", user.Password);

        cmd.ExecuteNonQuery();

        return user;
    }

    public User? Login(string username, string password)
    {
        var sql = @"SELECT * FROM users WHERE name = @username AND password = @password";
        using var cmd = new NpgsqlCommand(sql, this.connection);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@password", password);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        var user = new User
        {
            Id = reader.GetGuid(0),
            Name = reader.GetString(1),
            Password = reader.GetString(2),
        };

        loggedInUser = user.Id;

        return user;
    }

    public void Logout()
    {
        loggedInUser = null;
    }

    public User? GetLoggedInUser()
    {
        if (loggedInUser == null)
        {
            return null;
        }

        var sql = @"SELECT * FROM users WHERE user_id = @id";
        using var cmd = new NpgsqlCommand(sql, this.connection);
        cmd.Parameters.AddWithValue("@id", loggedInUser);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        var user = new User
        {
            Id = reader.GetGuid(0),
            Name = reader.GetString(1),
            Password = reader.GetString(2),
        };

        return user;
    }

    public void RemoveUser()
    {
        var user = GetLoggedInUser();

        var sql =
            @"
        BEGIN;
  DELETE FROM transactions 
  WHERE user_id = @userId;
  
  DELETE FROM users 
  WHERE user_id = @userId;
COMMIT;";
        using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@userId", user.Id);
        cmd.ExecuteNonQuery();
        Console.WriteLine($" Your account has been deleted!");
    }

    public int LäggTillTransaktion()
    {
        try
        {
            var currentUser = GetLoggedInUser();
            if (currentUser == null)
            {
                Console.WriteLine("Ingen användare är inloggad. Logga in för att fortsätta.");
                return 0;
            }

            Guid currentUserId = currentUser.Id;

            Console.WriteLine("Ange typ (income/expense):");
            string type = Console.ReadLine()?.ToLower();


            if (type != "income" && type != "expense")
            {
                Console.WriteLine("Ogiltig typ. Ange 'income' eller 'expense'.");
                return 0;
            }

            // Hämta belopp från användaren
            Console.WriteLine("Ange belopp:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                Console.WriteLine("Ogiltigt belopp. Ange ett nummer.");
                return 0;
            }

            string query =
                "INSERT INTO transactions (user_id, type, amount) VALUES (@user_id, @type, @amount)";

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("user_id", currentUserId);
            cmd.Parameters.AddWithValue("type", type);
            cmd.Parameters.AddWithValue("amount", amount);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected == 1)
            {
                Console.WriteLine("Transaktionen har lagts till.");
            }
            else
            {
                Console.WriteLine("Transaktionen kunde inte läggas till.");
            }

            return rowsAffected; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ett fel inträffade: {ex.Message}");
            return 0; 
        }
    }

    public void VisaSaldo()
    {
        try
        {
            var currentUser = GetLoggedInUser();
            if (currentUser == null)
            {
                Console.WriteLine("Ingen användare är inloggad.");
                return;
            }

            Guid currentUserId = currentUser.Id;

            string query = "SELECT SUM(amount) FROM transactions WHERE user_id = @user_id";

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("user_id", currentUserId);
            var result = cmd.ExecuteScalar();

            decimal saldo = result == DBNull.Value ? 0 : Convert.ToDecimal(result);
            Console.WriteLine($"Nuvarande saldo: {saldo}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ett fel inträffade: {ex.Message}");
        }
    }

    public void VisaUtgifter()
    {

        try
    {
        var currentUser = GetLoggedInUser();
        if (currentUser == null)
        {
            Console.WriteLine("Ingen användare är inloggad. Logga in för att fortsätta.");
            return;
        }

        string query = "SELECT id, type, amount, transaction_date FROM transactions WHERE user_id = @user_id AND type = @type";

        using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("user_id", currentUser.Id);
        cmd.Parameters.AddWithValue("type", "expense");

        using var reader = cmd.ExecuteReader();

        Console.WriteLine($"Alla 'expense'-transaktioner för användare {currentUser.Name}:");
        Console.WriteLine("---------------------------------------");

        bool hasResults = false;

        while (reader.Read())
        {
            hasResults = true;

            int id = reader.GetInt32(0);
            string type = reader.GetString(1);
            decimal amount = reader.GetDecimal(2); 
            DateTime transactionDate = reader.GetDateTime(3); 

            Console.WriteLine($"ID: {id}, Typ: {type}, Belopp: {amount}, Datum: {transactionDate}");
        }

        if (!hasResults)
        {
            Console.WriteLine("Inga 'expense'-transaktioner hittades för den inloggade användaren.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ett fel inträffade: {ex.Message}");
    }

    }

    public void VisaInkomster()
    {

        try
    {
        
        var currentUser = GetLoggedInUser();
        if (currentUser == null)
        {
            Console.WriteLine("Ingen användare är inloggad. Logga in för att fortsätta.");
            return; 
        }

        
        string query = "SELECT id, type, amount, transaction_date FROM transactions WHERE user_id = @user_id AND type = @type";

        using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("user_id", currentUser.Id);
        cmd.Parameters.AddWithValue("type", "income");

        using var reader = cmd.ExecuteReader();

        Console.WriteLine($"Alla 'income'-transaktioner för användare {currentUser.Name}:");
        Console.WriteLine("---------------------------------------");

        bool hasResults = false; 

        while (reader.Read())
        {
            hasResults = true;

            int id = reader.GetInt32(0); 
            string type = reader.GetString(1); 
            decimal amount = reader.GetDecimal(2); 
            DateTime transactionDate = reader.GetDateTime(3); 

            Console.WriteLine($"ID: {id}, Typ: {type}, Belopp: {amount}, Datum: {transactionDate}");
        }

        if (!hasResults)
        {
            Console.WriteLine("Inga 'income'-transaktioner hittades för den inloggade användaren.");
        }
    }

    catch (Exception ex)
    {
        Console.WriteLine($"Ett fel inträffade: {ex.Message}");
    }
    
    }

    public void AllaTransaktioner()
    {
        try
    {
        var currentUser = GetLoggedInUser();
        if (currentUser == null)
        {
            Console.WriteLine("Ingen användare är inloggad.");
            return;
        }

        Console.WriteLine("Välj tidsperiod för att visa transaktioner:");
        Console.WriteLine("1. Årsvis");
        Console.WriteLine("2. Månadsvis");
        Console.WriteLine("3. Veckovis");
        Console.WriteLine("4. Dagvis");
        string periodChoice = Console.ReadLine();

        if (periodChoice is not ("1" or "2" or "3" or "4"))
        {
            Console.WriteLine("Ogiltigt val. Försök igen.");
            return;
        }

        string query = periodChoice switch
        {
            "1" => "SELECT id, type, amount, transaction_date FROM transactions WHERE user_id = @user_id AND EXTRACT(YEAR FROM transaction_date) = EXTRACT(YEAR FROM CURRENT_DATE)",
            "2" => "SELECT id, type, amount, transaction_date FROM transactions WHERE user_id = @user_id AND EXTRACT(MONTH FROM transaction_date) = EXTRACT(MONTH FROM CURRENT_DATE)",
            "3" => "SELECT id, type, amount, transaction_date FROM transactions WHERE user_id = @user_id AND EXTRACT(WEEK FROM transaction_date) = EXTRACT(WEEK FROM CURRENT_DATE)",
            "4" => "SELECT id, type, amount, transaction_date FROM transactions WHERE user_id = @user_id AND transaction_date::DATE = CURRENT_DATE",
            _ => throw new Exception("Ogiltigt val")
        };

        using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("user_id", currentUser.Id);

        List<int> transactionIds = new List<int>();

        using var reader = cmd.ExecuteReader();

        bool hasResults = false;
        Console.WriteLine("Transaktioner:");
        Console.WriteLine("ID | Typ      | Belopp | Datum");
        Console.WriteLine("------------------------------------");

        while (reader.Read())
        {
            hasResults = true;
            int id = reader.GetInt32(0);
            string type = reader.GetString(1);
            decimal amount = reader.GetDecimal(2);
            DateTime date = reader.GetDateTime(3);

            transactionIds.Add(id);

            Console.WriteLine($"{id} | {type,-8} | {amount,7} | {date:yyyy-MM-dd}");
        }

        if (!hasResults)
        {
            Console.WriteLine("Inga transaktioner hittades för vald tidsperiod.");
            return;
        }

        reader.Close();

        Console.WriteLine("\nVill du radera en transaktion? (ja/nej)");
        string deleteChoice = Console.ReadLine()?.ToLower();

        if (deleteChoice == "ja")
        {
            Console.WriteLine("Ange ID på transaktionen du vill radera:");
            if (int.TryParse(Console.ReadLine(), out int transactionId) && transactionIds.Contains(transactionId))
            {
                string deleteQuery = "DELETE FROM transactions WHERE id = @id AND user_id = @user_id";

                using var deleteCmd = new NpgsqlCommand(deleteQuery, connection);
                deleteCmd.Parameters.AddWithValue("id", transactionId);
                deleteCmd.Parameters.AddWithValue("user_id", currentUser.Id);

                int rowsDeleted = deleteCmd.ExecuteNonQuery();

                if (rowsDeleted > 0)
                {
                    Console.WriteLine($"Transaktionen med ID {transactionId} har raderats.");
                }
                else
                {
                    Console.WriteLine("Raderingen misslyckades. Kontrollera att ID:t är korrekt.");
                }
            }
            else
            {
                Console.WriteLine("Ogiltigt eller obefintligt ID.");
            }
        }
        else
        {
            Console.WriteLine("Ingen transaktion raderades.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ett fel inträffade: {ex.Message}");
    }
}

/*public void RaderaTransaktion(int transactionId, Guid userId)
{
    try
    {
        string query = "DELETE FROM transactions WHERE id = @id AND user_id = @user_id";

        using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("id", transactionId);
        cmd.Parameters.AddWithValue("user_id", userId);

        int rowsAffected = cmd.ExecuteNonQuery();

        if (rowsAffected > 0)
        {
            Console.WriteLine("Transaktionen har raderats.");
        }
        else
        {
            Console.WriteLine("Ingen transaktion hittades med det angivna ID:t.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ett fel inträffade vid radering: {ex.Message}");
    }
    }*/
    public NpgsqlConnection GetConnection()
    {
        throw new NotImplementedException();
    }
}
