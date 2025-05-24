using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControleAcessoApi.Data;
using ControleAcessoApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace ControleAcessoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("me/claims")]
        [Authorize]
        public IActionResult GetMeClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }
        // POST - Usuário padrão cria seu cadastro (não aprova na criação)
        [HttpPost]
        //[Authorize(Roles = "Admin")] // Comentei para liberar criação para usuários padrão
        public IActionResult CreateUsuario([FromBody] Usuario usuario)
        {
            // NÃO aprova automaticamente o usuário criado
            // usuario.IsAprovado = true;

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            // Retorna o usuário criado (com senha — você pode querer limpar ou retornar DTO aqui)
            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

        // GET - Admin pode baixar todos usuários sem senha e com info do aprovador
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUsuarios()
        {
            var usuarios = _context.Usuarios
                //.Where(u => u.IsAprovado) // Opcional: filtrar só aprovados
                .Select(u => new
                {
                    u.Id,
                    u.Nome,
                    u.Email,
                    u.Idade,
                    u.Role,
                    u.IsAprovado,
                    Aprovador = _context.Usuarios
                        .Where(ap => ap.Id == u.AprovadorId)
                        .Select(ap => ap.Nome)
                        .FirstOrDefault()
                })
                .ToList();

            return Ok(usuarios);
        }

        // GET usuario por id (para aprovar ou ver detalhes) — acessível por Admin e Aprovador
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Aprovador")]
        public IActionResult GetUsuario(int id)
        {
            var usuario = _context.Usuarios
                .Include(u => u.UsuarioAcessos)
                .ThenInclude(ua => ua.Acesso)
                .FirstOrDefault(u => u.Id == id);

            if (usuario == null)
                return NotFound();

            // Para segurança, não retornar senha diretamente:
            return Ok(new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                usuario.Idade,
                usuario.Role,
                usuario.IsAprovado,
                usuario.AprovadorId,
                UsuarioAcessos = usuario.UsuarioAcessos.Select(ua => new
                {
                    ua.Acesso!.AcessoId,
                    ua.Acesso.Nome
                })
            });
        }

        [HttpGet("pendentes")]
        [Authorize(Roles = "Admin,Aprovador")]
        public IActionResult GetUsuariosPendentes()
        {
            var emailLogado = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(emailLogado))
                return Unauthorized();

            var dominioLogado = emailLogado.Split('@').Last().ToLower();

            var pendentes = _context.Usuarios
                .Where(u => !u.IsAprovado && u.Email.EndsWith("@" + dominioLogado))
                .Select(u => new
                {
                    u.Id,
                    u.Nome,
                    u.Email
                })
                .ToList();

            return Ok(pendentes);
        }


        // PUT - Aprovar usuário (Admin e Aprovador só podem aprovar usuários do mesmo domínio)
        [HttpPut("{id}/aprovar")]
        [Authorize(Roles = "Aprovador,Admin")]
        public IActionResult AprovarUsuario(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
                return NotFound();

            // Pega o email do usuário logado via claims
            var emailLogado = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(emailLogado))
                return Unauthorized("Email do usuário logado não encontrado.");

            var usuarioLogado = _context.Usuarios.FirstOrDefault(u => u.Email == emailLogado);
            if (usuarioLogado == null)
                return Unauthorized();

            // Verifica se o domínio do email do aprovador/admin e do usuário são iguais
            var dominioUsuario = usuario.Email!.Split('@').Last().ToLower();
            var dominioLogado = usuarioLogado.Email!.Split('@').Last().ToLower();

            if (dominioUsuario != dominioLogado)
            {
                return Forbid("Você só pode aprovar usuários do mesmo domínio da sua empresa.");
            }

            usuario.IsAprovado = true;
            usuario.AprovadorId = usuarioLogado.Id;
            _context.SaveChanges();

            return Ok(new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                usuario.Idade,
                usuario.Role,
                usuario.IsAprovado,
                usuario.AprovadorId
            });
        }

        // DELETE - apenas Admin pode excluir usuário
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUsuario(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
                return NotFound();

            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE api/usuarios/limpar
        [HttpDelete("limpar")]
        [Authorize(Roles = "Admin")]  // Somente Admin pode executar essa ação destrutiva
        public IActionResult DeleteAllUsuarios()
        {
            var usuarios = _context.Usuarios.ToList();

            if (!usuarios.Any())
                return NoContent(); // ou Ok("Nenhum usuário para deletar.");

            _context.Usuarios.RemoveRange(usuarios);
            _context.SaveChanges();

            return Ok("Todos os usuários foram deletados.");
        }

    }
}
