using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ControleAcessoApi.Models;
using ControleAcessoApi.Data;

namespace ControleAcessoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            // Etapa 1: Buscar apenas pelo email
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == loginDto.Email);

            // Verifica se o email existe
            if (usuario == null)
            {
                return Unauthorized("Usuário não está cadastrado.");
            }

            // Verifica se o usuário está aprovado
            if (!usuario.IsAprovado)
            {
                return Unauthorized("Usuário ainda não aprovado. Aguarde aprovação.");
            }

            // Verifica se a senha está correta
            if (usuario.Senha != loginDto.Senha)
            {
                return Unauthorized("Email ou senha inválidos.");
            }

            // Autenticação válida — gera o token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Role),
                new Claim("UsuarioId", usuario.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

    }
}
