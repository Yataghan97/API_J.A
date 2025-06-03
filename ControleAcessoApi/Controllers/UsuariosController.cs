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

        [HttpPost]
        public IActionResult CreateUsuario([FromBody] Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUsuarios()
        {
            var usuarios = _context.Usuarios
                .GroupJoin(
                    _context.Usuarios,
                    u => u.AprovadorId,
                    a => a.Id,
                    (u, aprovadores) => new { u, aprovadores }
                )
                .SelectMany(
                    ua => ua.aprovadores.DefaultIfEmpty(),
                    (ua, aprovador) => new
                    {
                        ua.u.Id,
                        ua.u.Nome,
                        ua.u.Email,
                        ua.u.Idade,
                        ua.u.Role,
                        ua.u.IsAprovado,
                        Aprovador = aprovador != null ? aprovador.Nome : null
                    }
                )
                .ToList();

            return Ok(usuarios);
        }

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

        [HttpPut("{id}/aprovar")]
        [Authorize(Roles = "Admin,Aprovador")]
        public IActionResult AprovarUsuario(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
                return NotFound();

            var emailLogado = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(emailLogado))
                return Unauthorized("Email do usuário logado não encontrado.");

            var usuarioLogado = _context.Usuarios.FirstOrDefault(u => u.Email == emailLogado);
            if (usuarioLogado == null)
                return Unauthorized();

            var dominioUsuario = usuario.Email!.Split('@').Last().ToLower();
            var dominioLogado = usuarioLogado.Email!.Split('@').Last().ToLower();

            if (usuarioLogado.Role != "Admin" && dominioUsuario != dominioLogado)
            {
                return Forbid("Você só pode aprovar usuários do mesmo domínio.");
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

        // Nova rota para negar usuário
        [HttpPut("{id}/negar")]
        [Authorize(Roles = "Admin,Aprovador")]
        public IActionResult NegarUsuario(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
                return NotFound();

            var emailLogado = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(emailLogado))
                return Unauthorized("Email do usuário logado não encontrado.");

            var usuarioLogado = _context.Usuarios.FirstOrDefault(u => u.Email == emailLogado);
            if (usuarioLogado == null)
                return Unauthorized();

            var dominioUsuario = usuario.Email!.Split('@').Last().ToLower();
            var dominioLogado = usuarioLogado.Email!.Split('@').Last().ToLower();

            if (usuarioLogado.Role != "Admin" && dominioUsuario != dominioLogado)
            {
                return Forbid("Você só pode negar usuários do mesmo domínio.");
            }

            usuario.IsAprovado = false;
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

        [HttpDelete("limpar")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteAllUsuarios()
        {
            var usuarios = _context.Usuarios.ToList();
            if (!usuarios.Any())
                return NoContent();

            _context.Usuarios.RemoveRange(usuarios);
            _context.SaveChanges();

            return Ok("Todos os usuários foram deletados.");
        }

        [HttpGet("usuarios_dominio")]
        [Authorize(Roles = "Admin,Aprovador")]
        public IActionResult GetUsuariosDoMesmoDominio()
        {
            var emailLogado = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(emailLogado))
                return Unauthorized();

            var dominioLogado = emailLogado.Split('@').Last().ToLower();

            var usuariosDoMesmoDominio = _context.Usuarios
                .Where(u => u.Email.EndsWith("@" + dominioLogado))
                .GroupJoin(
                    _context.Usuarios,
                    u => u.AprovadorId,
                    a => a.Id,
                    (u, aprovadores) => new { u, aprovadores }
                )
                .SelectMany(
                    ua => ua.aprovadores.DefaultIfEmpty(),
                    (ua, aprovador) => new
                    {
                        ua.u.Id,
                        ua.u.Nome,
                        ua.u.Email,
                        ua.u.Idade,
                        ua.u.Role,
                        ua.u.IsAprovado,
                        Aprovador = aprovador != null ? aprovador.Nome : null
                    }
                )
                .ToList();

            return Ok(usuariosDoMesmoDominio);
        }

        [HttpGet("dominios")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetDominios()
        {
            var dominios = _context.Usuarios
                .AsEnumerable()  // trazer para memória para permitir Split e Last
                .Select(u => u.Email.Split('@').Last().ToLower())
                .Distinct()
                .ToList();

            return Ok(dominios);
        }

        [HttpGet("dominio/{dominio}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUsuariosPorDominio(string dominio)
        {
            if (string.IsNullOrEmpty(dominio))
                return BadRequest("Domínio inválido.");

            var usuarios = _context.Usuarios
                .Where(u => u.Email.ToLower().EndsWith("@" + dominio.ToLower()))
                .GroupJoin(
                    _context.Usuarios,
                    u => u.AprovadorId,
                    a => a.Id,
                    (u, aprovadores) => new { u, aprovadores }
                )
                .SelectMany(
                    ua => ua.aprovadores.DefaultIfEmpty(),
                    (ua, aprovador) => new
                    {
                        ua.u.Id,
                        ua.u.Nome,
                        ua.u.Email,
                        ua.u.Idade,
                        ua.u.Role,
                        ua.u.IsAprovado,
                        Aprovador = aprovador != null ? aprovador.Nome : null
                    }
                )
                .ToList();

            return Ok(usuarios);
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetMeuPerfil()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var usuario = _context.Usuarios
                .Include(u => u.UsuarioAcessos)
                .ThenInclude(ua => ua.Acesso)
                .FirstOrDefault(u => u.Email == email);

            if (usuario == null)
                return NotFound();

            if (!usuario.IsAprovado)
                return Forbid("Aguardando aprovação.");

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
    }
}
