using ControleAcessoApi.Models;  // Adicionando a diretiva using correta

namespace ControleAcessoApi.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        
        // Relacionamento com UsuarioAcesso
        public ICollection<UsuarioAcesso> UsuarioAcessos { get; set; } = new List<UsuarioAcesso>();
    }
}
