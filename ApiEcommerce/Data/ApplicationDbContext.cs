using ApiEcommerce.Models;
using Microsoft.EntityFrameworkCore;

/* 
    - dotnet ef migrations add InitialCreate: Crea una nueva migración llamada "InitialCreate" que contiene las instrucciones para crear las tablas y 
    relaciones basadas en los modelos definidos en el contexto de la base de datos.
    - dotnet ef database update: Aplica la migración más reciente a la base de datos, creando las tablas y relaciones según lo definido en la migración.
    - dotnet ef migrations list: Lista todas las migraciones que se han creado para el proyecto.
    - dotnet ef migrations remove: Elimina la última migración creada si aún no ha sido aplicada a la base de datos.
    - dotnet ef database update: Aplica todas las migraciones pendientes a la base de datos.
*/
public class ApplicationDbContext : DbContext // DbContext es una clase base para interactuar con la base de datos
{
    // Constructor que recibe las opciones de configuración para el contexto de la base de datos
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Category> Categories { get; set; } // DbSet representa una colección de entidades que se pueden consultar y guardar en la base de datos
    public DbSet<Product> Products { get; set; }
}