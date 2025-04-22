// ApplicationDbContext.cs
namespace ControleAcessoApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using ControleAcessoApi.Models;

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Acesso> Acessos { get; set; }
        public DbSet<UsuarioAcesso> UsuarioAcessos { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UsuarioAcesso>()
                .HasKey(ua => new { ua.UsuarioId, ua.AcessoId });  // Chave composta

            modelBuilder.Entity<UsuarioAcesso>()
                .HasOne(ua => ua.Usuario)
                .WithMany(u => u.UsuarioAcessos)
                .HasForeignKey(ua => ua.UsuarioId);

            modelBuilder.Entity<UsuarioAcesso>()
                .HasOne(ua => ua.Acesso)
                .WithMany(a => a.UsuarioAcessos)
                .HasForeignKey(ua => ua.AcessoId);
        }
    }
}
