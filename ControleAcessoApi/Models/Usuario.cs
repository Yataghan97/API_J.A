using System.Collections.Generic;
using ControleAcessoApi.Models;
using System.ComponentModel.DataAnnotations;


public class Usuario
{
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; }

    public ICollection<UsuarioAcesso> UsuarioAcessos { get; set; } = new List<UsuarioAcesso>();
}
