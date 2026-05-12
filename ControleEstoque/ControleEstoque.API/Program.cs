using ControleEstoque.API.Data;
using ControleEstoque.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// CONEXÃO SQL SERVER
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")
    ));

// SERVICES
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IFornecedorService, FornecedorService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IContaReceberService, ContaReceberService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

// TOKEN SERVICE
builder.Services.AddScoped<ITokenService, TokenService>();

// CONTROLLERS
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Evita referência cíclica
    options.JsonSerializerOptions.ReferenceHandler =
        ReferenceHandler.IgnoreCycles;
});

// JWT AUTHENTICATION
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme
)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer =
                builder.Configuration["Jwt:Issuer"],

            ValidAudience =
                builder.Configuration["Jwt:Audience"],

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        builder.Configuration["Jwt:Key"]!
                    )
                )
        };
});

// AUTHORIZATION
builder.Services.AddAuthorization();

// SWAGGER
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options => // Configurações do Swagger
{
    options.SwaggerDoc("v1", new OpenApiInfo // Informações da API
    {
        Title = "ControleEstoque API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme // Definição do esquema de segurança para JWT
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http, // Tipo HTTP para autenticação
        Scheme = "bearer", // Esquema de autenticação Bearer
        BearerFormat = "JWT", // Formato do token JWT
        In = ParameterLocation.Header, // O token será enviado no cabeçalho da requisição
        Description = "Digite: Bearer {seu token}" // Descrição para o usuário sobre como usar o token
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement // Requisito de segurança para proteger as rotas da API
    {
        {
            new OpenApiSecurityScheme // Referência ao esquema de segurança definido acima
            {
                Reference = new OpenApiReference // Referência para o esquema de segurança
                {
                    Type = ReferenceType.SecurityScheme, // Tipo de referência para esquema de segurança
                    Id = "Bearer" // ID do esquema de segurança definido acima
                }
            },
            Array.Empty<string>() // Lista de escopos (vazia neste caso, pois não estamos usando escopos específicos)
        }
    });
});




var app = builder.Build();

// SWAGGER
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// JWT
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();