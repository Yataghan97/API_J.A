using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ControleAcessoApi.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public bool IsAprovado { get; set; } = false;

        public int? AprovadorId { get; set; }  // FK para usuário que aprovou

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Senha { get; set; } = string.Empty;

        public int Idade { get; set; }

        [Required]
        public string Role { get; set; } = "Padrao"; // "Admin", "Aprovador", "Padrao"

        public ICollection<UsuarioAcesso> UsuarioAcessos { get; set; } = new List<UsuarioAcesso>();
    }
}
