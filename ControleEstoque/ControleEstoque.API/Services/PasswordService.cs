namespace ControleEstoque.API.Services

{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password) // Gera o hash da senha usando BCrypt
        {
            return BCrypt.Net.BCrypt.HashPassword(password); // O método HashPassword do BCrypt gera um hash seguro da senha fornecida
        }

        public bool VerifyPassword(string password, string passwordHash) // Verifica se a senha fornecida corresponde ao hash armazenado
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash); // O método Verify do BCrypt compara a senha fornecida com o hash armazenado e retorna true se corresponderem, ou false caso contrário
        }
    }
}
