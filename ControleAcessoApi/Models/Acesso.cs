using System.Collections.Generic;

namespace ControleAcessoApi.Models
{
    public class Acesso
    {
        public int AcessoId { get; set; }
        public required string? Nome { get; set; }
        public ICollection<UsuarioAcesso> UsuarioAcessos { get; set; } = new List<UsuarioAcesso>();
    }
}
