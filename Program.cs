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

// 2. Configurar la base de datos SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();

// OBLIGATORIO: Activar CORS antes de los endpoints
app.UseCors("PermitirTodo");

// Asegurar la creación de la base de datos al arrancar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// ----------------------------------------------------
// ENDPOINTS DE LA API
// ----------------------------------------------------

// Endpoint de prueba / raíz
app.MapGet("/", () => "API de Tienda activa y funcionando correctamente");

// Endpoint para consultar productos
app.MapGet("/api/productos", async (AppDbContext db) =>
{
    return await db.Productos.ToListAsync();
});

// Endpoint de Inicio de Sesión (Login)
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

// ----------------------------------------------------
// MODELO DE DATOS PARA LOGIN (DTO)
// ----------------------------------------------------
public class UsuarioLogin
{
    public string Usuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}