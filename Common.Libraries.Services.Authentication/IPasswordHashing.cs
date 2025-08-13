namespace Common.Libraries.Services.Authentication
{
    public interface IPasswordHashing
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}