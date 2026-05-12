using ControleEstoque.API.Data;
using ControleEstoque.API.DTOs;
using ControleEstoque.API.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleEstoque.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context; // Injeção do contexto para acessar o banco de dados
        private readonly ITokenService _tokenService; // Injeção do serviço de token para gerar JWT
        private readonly IPasswordService _passwordService; // Injeção do serviço de senha para verificar a senha

        public AuthController( // Construtor para injetar as dependências
            AppDbContext context,
            ITokenService tokenService,
            IPasswordService passwordService)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordService = passwordService; 
        }

        [HttpPost("login")] // Rota para login
        public async Task<IActionResult> Login(LoginDTO dto) // Método para autenticar o usuário
        {
            var usuario = await _context.Usuarios // Consulta para encontrar o usuário pelo email
                .FirstOrDefaultAsync(x =>
                    x.Email == dto.Email);

            if (usuario == null)
            {
                return Unauthorized("Usuário não encontrado");
            }

            // COMPARAÇÃO SIMPLES
            // depois você pode usar BCrypt

            if (!_passwordService.VerifyPassword(dto.Senha, usuario.SenhaHash))
            {
                return Unauthorized("Senha inválida");
            }

            var token = _tokenService.GenerateToken(usuario);

            return Ok(new
            {
                token
            });
        }
    }
}