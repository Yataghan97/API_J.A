namespace ControleAcessoApi

{
    using Microsoft.EntityFrameworkCore;
    using ControleAcessoApi.Data;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuração do banco de dados SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Recupera a chave JWT do appsettings.json e valida se foi configurada
            var jwtKey = builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("A chave JWT ('Jwt:Key') não foi configurada.");

            // Configuração de autenticação JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            // Adicionando controladores
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            // Constrói o app
            var app = builder.Build();

            // Configuração do pipeline de requisições
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
                
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();  // Habilita a autenticação JWT
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
