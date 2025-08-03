using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext // DbContext es una clase base para interactuar con la base de datos
{
    // Constructor que recibe las opciones de configuración para el contexto de la base de datos
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Category> Categories { get; set; } // DbSet representa una colección de entidades que se pueden consultar y guardar en la base de datos
}