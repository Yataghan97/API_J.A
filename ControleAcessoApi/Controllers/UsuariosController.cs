using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControleAcessoApi.Data;
using ControleAcessoApi.Models;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost]
        [AllowAnonymous] // Libera o cadastro de usuário sem precisar de token
        public IActionResult CreateUsuario([FromBody] Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }


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

            return Ok(usuario);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetUsuarios()
        {
            var usuarios = _context.Usuarios
                .Include(u => u.UsuarioAcessos)
                .ThenInclude(ua => ua.Acesso) 
                .ToList();

            return Ok(usuarios);
}


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Aprovador")]
        public IActionResult UpdateUsuario(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.Id)
                return BadRequest();

            _context.Entry(usuario).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

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
    }
}
