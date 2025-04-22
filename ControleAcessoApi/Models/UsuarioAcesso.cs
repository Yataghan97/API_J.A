using ControleAcessoApi.Models;

namespace ControleAcessoApi.Models
{
    public class UsuarioAcesso
    {
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int AcessoId { get; set; }
        public Acesso Acesso { get; set; }
    }
}
