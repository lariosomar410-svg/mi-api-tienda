using Microsoft.EntityFrameworkCore;

// 1. Representa la sesión con la base de datos
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // 2. Tabla de productos en la base de datos
    public DbSet<Producto> Productos { get; set; }
}

// 3. Modelo del Producto
public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}