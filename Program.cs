using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 2. Configurar SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();

// Activar CORS
app.UseCors("PermitirTodo");

// Inicializar base de datos
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "Teclado Mecánico", Precio = 49.99m },
            new Producto { Nombre = "Mouse Gamer", Precio = 25.50m },
            new Producto { Nombre = "Monitor 24 pulgadas", Precio = 120.00m }
        );
        db.SaveChanges();
    }
}

// ----------------------------------------------------
// ENDPOINTS DE LA API
// ----------------------------------------------------

// 1. Raíz de prueba
app.MapGet("/", () => "API de Tienda activa");

// 2. Obtener productos (GET)
app.MapGet("/api/productos", async (AppDbContext db) =>
{
    return await db.Productos.ToListAsync();
});

// 3. Crear producto (POST)
app.MapPost("/api/productos", async (AppDbContext db, Producto producto) =>
{
    db.Productos.Add(producto);
    await db.SaveChangesAsync();
    return Results.Created($"/api/productos/{producto.Id}", producto);
});

// 4. Eliminar producto (DELETE)
app.MapDelete("/api/productos/{id}", async (AppDbContext db, int id) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    db.Productos.Remove(producto);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// 5. Endpoint de Login
app.MapPost("/api/login", (UsuarioLogin login) =>
{
    if (login != null && 
        string.Equals(login.Usuario, "admin", StringComparison.OrdinalIgnoreCase) && 
        login.Password == "1234")
    {
        return Results.Ok(new 
        { 
            token = "jwt-fake-token-12345", 
            usuario = login.Usuario,
            mensaje = "Inicio de sesión exitoso" 
        });
    }

    return Results.Unauthorized();
});

app.Run();

// DTO para el Login únicamente
public class UsuarioLogin
{
    public string Usuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}