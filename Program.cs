using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar CORS para permitir peticiones desde el Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirVue", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 2. Registrar la base de datos SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();

// Usar middleware de CORS
app.UseCors("PermitirVue");

// 3. Crear la base de datos automáticamente al iniciar si no existe
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// -------------------------------------------------------------
// ENDPOINTS DE LA API REST (CRUD + VALIDACIONES)
// -------------------------------------------------------------

// GET: Obtener todos los productos
app.MapGet("/api/productos", async (AppDbContext db) =>
    await db.Productos.ToListAsync());

// POST: Agregar un nuevo producto (con validación)
app.MapPost("/api/productos", async (AppDbContext db, Producto nuevoProducto) =>
{
    if (string.IsNullOrWhiteSpace(nuevoProducto.Nombre) || nuevoProducto.Precio <= 0)
    {
        return Results.BadRequest("El nombre no puede estar vacío y el precio debe ser mayor a 0.");
    }

    db.Productos.Add(nuevoProducto);
    await db.SaveChangesAsync();
    return Results.Created($"/api/productos/{nuevoProducto.Id}", nuevoProducto);
});

// PUT: Actualizar un producto existente (con validación)
app.MapPut("/api/productos/{id}", async (AppDbContext db, int id, Producto productoActualizado) =>
{
    if (string.IsNullOrWhiteSpace(productoActualizado.Nombre) || productoActualizado.Precio <= 0)
    {
        return Results.BadRequest("El nombre no puede estar vacío y el precio debe ser mayor a 0.");
    }

    var prod = await db.Productos.FindAsync(id);
    if (prod is null) return Results.NotFound("Producto no encontrado");

    prod.Nombre = productoActualizado.Nombre;
    prod.Precio = productoActualizado.Precio;

    await db.SaveChangesAsync();
    return Results.Ok(prod);
});

// DELETE: Eliminar un producto por ID
app.MapDelete("/api/productos/{id}", async (AppDbContext db, int id) =>
{
    var prod = await db.Productos.FindAsync(id);
    if (prod is null) return Results.NotFound("Producto no encontrado");

    db.Productos.Remove(prod);
    await db.SaveChangesAsync();
    return Results.Ok(new { mensaje = "Producto eliminado con éxito" });
});

app.Run();


// POST: Endpoint de Login para autenticación
app.MapPost("/api/login", (UsuarioLogin login) =>
{
    // Validación básica de credenciales (puedes ajustar el usuario/password que gustes)
    if (login.Usuario == "admin" && login.Password == "1234")
    {
        // Retorna un token ficticio o la respuesta exitosa que espera tu frontend
        return Results.Ok(new { token = "jwt-token-ficticio-de-prueba", mensaje = "Login exitoso" });
    }

    return Results.Unauthorized();
});

// Modelo de datos para recibir las credenciales
record UsuarioLogin(string Usuario, string Password);