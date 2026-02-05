// Este es el punto de entrada para cualquier aplicación en ASP.NET Core.
using System.Text;
using ApiEcommerce.Constants;
using ApiEcommerce.Repository;
using ApiEcommerce.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbConnectionString = builder.Configuration.GetConnectionString("ConexionSql");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnectionString));

// Se configura el servicio de caché

builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024; // Tamaño máximo del cuerpo en bytes para almacenar en caché
    options.UseCaseSensitivePaths = true; // Las rutas serán sensibles a mayúsculas y minúsculas
});

// Se realiza la inyección de dependencias para los repositorios
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); // Inyección de dependencias para CategoryRepository
builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Inyección de dependencias para ProductRepository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(Program).Assembly);
}); // Configuración de AutoMapper para mapear entre entidades y DTOs

// Configuración de autenticación
var secreteKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");

if (string.IsNullOrEmpty(secreteKey))
{
    throw new InvalidOperationException("La SecretKey no está configurada");
}

builder.Services.AddAuthentication( options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Especifica el esquema de autenticación predeterminado
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Especifica el esquema de desafío predeterminado
}).AddJwtBearer( options =>
{
    options.RequireHttpsMetadata = false; // En producción, esto debería ser true para mayor seguridad
    options.SaveToken = true; // Guarda el token en el contexto de la solicitud
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // Valida la clave de firma del emisor
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secreteKey)), // Clave secreta para validar la firma del token
        ValidateIssuer = false, // No valida el emisor
        ValidateAudience = false, // No valida la audiencia
    };
});


builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); // 	Detecta tus endpoints para que se puedan documentar automáticamente

// Genera el archivo Swagger (OpenAPI) y te da la UI para verlo y probar la API
builder.Services.AddSwaggerGen(
    options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Nuestra API utiliza la Autenticación JWT usando el esquema Bearer. \n\r\n\r" +
                          "Ingrese la palabra a continuación el token generado en login.\n\r\n\r" +
                          "Ejemplo: \"12345abcdef\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer"
        });
        options.AddSecurityRequirement( new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
    }
); 

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("*") // Permitir cualquier origen
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Estos de aquí abajo son middleware que se ejecutan en cada petición HTTP
app.UseHttpsRedirection();

app.UseCors(PolicyNames.AllowSpecificOrigin);

app.UseResponseCaching(); // Habilita el middleware de caché de respuestas

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
