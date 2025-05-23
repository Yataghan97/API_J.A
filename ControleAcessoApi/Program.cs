using Microsoft.EntityFrameworkCore;
using ControleAcessoApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace ControleAcessoApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Banco de dados
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // JWT Key e Configurações
            var jwtKey = builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("A chave JWT ('Jwt:Key') não foi configurada.");

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name,

                    ClockSkew = TimeSpan.FromMinutes(5)  // margem para diferença de horário
                };
            });

            // CORS (para front-end localhost)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthentication();

            // Middleware para DEBUG - mostrar claims no console
            app.Use(async (context, next) =>
            {
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    Console.WriteLine("Usuário autenticado com claims:");
                    foreach (var claim in context.User.Claims)
                    {
                        Console.WriteLine($"{claim.Type}: {claim.Value}");
                    }
                }
                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();


            app.Run();
        }
    }
}
