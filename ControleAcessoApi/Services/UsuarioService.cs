using ControleAcessoApi.Data;
using ControleAcessoApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API_Knapp.Services
{
    public class UsuarioService
    {
        private readonly ApplicationDbContext _context;

        public UsuarioService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retorna a lista de usuários
        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // Retorna um usuário pelo ID, ou null caso não encontrado
        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        // Adiciona um novo usuário
        public async Task AddUsuarioAsync(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario), "O usuário não pode ser nulo.");
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        // Atualiza um usuário existente
        public async Task UpdateUsuarioAsync(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario), "O usuário não pode ser nulo.");
            }

            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Deleta um usuário pelo ID
        public async Task DeleteUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                // Lança uma exceção se o usuário não for encontrado
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
