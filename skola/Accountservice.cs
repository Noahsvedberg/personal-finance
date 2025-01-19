using Npgsql;

public interface IAccountService
{
    User RegisterUser(string username, string password);
    User? Login(string username, string password);
    void Logout();
    User? GetLoggedInUser();
    void RemoveUser();
    void VisaSaldo();
    int LäggTillTransaktion();
    void VisaUtgifter();
    void VisaInkomster();
    void AllaTransaktioner();
    /*void RaderaTransaktion(int transactionId, Guid userId);*/
}
