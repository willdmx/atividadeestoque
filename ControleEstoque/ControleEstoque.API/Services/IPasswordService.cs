namespace ControleEstoque.API.Services
{
    public interface IPasswordService
    {
        string HashPassword(string password); // Método para gerar o hash da senha
        bool VerifyPassword(string password, string passwordHash); // Método para verificar se a senha corresponde ao hash
    }
}
