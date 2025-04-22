// Acesso.cs
namespace ControleAcessoApi.Models
{
    public class Acesso
    {
        public int AcessoId { get; set; }
        public string Nome { get; set; }
        public ICollection<UsuarioAcesso> UsuarioAcessos { get; set; }
    }
}
