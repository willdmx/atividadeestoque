using ControleEstoque.API.Models;

namespace ControleEstoque.API.Services
{
    public interface ITokenService
    {
        string GenerateToken(Usuario usuario);
    }
}