using ControleEstoque.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ControleEstoque.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

          // serve para gerar um token JWT (JSON Web Token) para um usuário autenticado.
         // O token inclui informações sobre o usuário (claims)
        // e é assinado usando uma chave secreta para garantir sua integridade e autenticidade.
        public string GenerateToken(Usuario usuario) 
        {
            var secretKey = _configuration["Jwt:Key"]; // serve para obter a chave secreta usada para assinar o token JWT a partir da configuração da aplicação. 
                                                       

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey!) //A classe SymmetricSecurityKey é usada para criar uma chave de segurança simétrica a partir da chave secreta obtida.
            );

            var credentials = new SigningCredentials( //A classe SigningCredentials é usada para criar as credenciais de assinatura para o token JWT
                key,
                SecurityAlgorithms.HmacSha256 //Especifica o algoritmo de assinatura HMAC SHA256 para garantir a integridade do token.
            );

            // CLAIMS (Payload) serve para armazenar informações sobre o usuário, como ID, email, nome e perfil.
            var claims = new[]
            {
                new Claim("Id", usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, usuario.Perfil.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}