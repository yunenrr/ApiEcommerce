// Este es el punto de entrada para cualquier aplicación en ASP.NET Core.
using ApiEcommerce.Constants;
using ApiEcommerce.Repository;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbConnectionString = builder.Configuration.GetConnectionString("ConexionSql");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnectionString));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); // Inyección de dependencias para CategoryRepository
builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Inyección de dependencias para ProductRepository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(cfg => {
    cfg.AddMaps(typeof(Program).Assembly);
}); // Configuración de AutoMapper para mapear entre entidades y DTOs
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); // 	Detecta tus endpoints para que se puedan documentar automáticamente
builder.Services.AddSwaggerGen(); // Genera el archivo Swagger (OpenAPI) y te da la UI para verlo y probar la API

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

app.UseAuthorization();

app.MapControllers();

app.Run();
