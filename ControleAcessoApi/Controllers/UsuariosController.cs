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
        [HttpGet("{id}")]
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

        // PUT - Aprovar usuário (somente aprovador pode aprovar usuários do mesmo domínio)
        [HttpPut("{id}/aprovar")]
        [Authorize(Roles = "Aprovador")]
        public IActionResult AprovarUsuario(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
                return NotFound();

            // Pega o email do aprovador (usuário logado) via claims (melhor forma)
            var aprovadorEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(aprovadorEmail))
                return Unauthorized("Email do aprovador não encontrado no token.");

            var aprovador = _context.Usuarios.FirstOrDefault(u => u.Email == aprovadorEmail);
            if (aprovador == null)
                return Unauthorized();

            // Verifica se o domínio do email do aprovador e do usuário são iguais
            var dominioUsuario = usuario.Email!.Split('@').Last().ToLower();
            var dominioAprovador = aprovador.Email!.Split('@').Last().ToLower();

            if (dominioUsuario != dominioAprovador)
            {
                return Forbid("Você só pode aprovar usuários do mesmo domínio da sua empresa.");
            }

            usuario.IsAprovado = true;
            usuario.AprovadorId = aprovador.Id;
            _context.SaveChanges();

            // Retorna dados do usuário aprovado (sem senha)
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
        [HttpDelete("{id}")]
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
