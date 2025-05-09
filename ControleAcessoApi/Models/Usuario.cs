using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ControleAcessoApi.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }

        public int Idade { get; set; }

        [Required]
        public string Role { get; set; } // "Admin", "Aprovador", "Padrao"

        public ICollection<UsuarioAcesso> UsuarioAcessos { get; set; } = new List<UsuarioAcesso>();
    }
}
