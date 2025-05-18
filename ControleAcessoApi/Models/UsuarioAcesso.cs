namespace ControleAcessoApi.Models
{
    public class UsuarioAcesso
    {
        public int UsuarioId { get; set; }
        public int AcessoId { get; set; }

        public required Usuario? Usuario { get; set; }
        public required Acesso? Acesso { get; set; }
    }
}
