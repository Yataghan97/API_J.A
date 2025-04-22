using ControleAcessoApi.Models; // Para garantir que a classe UsuarioAcesso seja reconhecida


namespace ControleAcessoApi.Models
{
    public class UsuarioAcesso
    {
        public int UsuarioId { get; set; }
        public int AcessoId { get; set; }
        public Usuario Usuario { get; set; }
        public Acesso Acesso { get; set; }
    }
}
