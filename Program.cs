using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Clave secreta para firmar tokens JWT (mínimo 16 caracteres)
var jwtSecretKey = "ClaveSecretaSuperSeguraParaTienda2026!";
var keyBytes = Encoding.UTF8.GetBytes(jwtSecretKey);

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

// 2. Configurar Autenticación y Autorización JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

// 3. Configurar SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();

// Middleware
app.UseCors("PermitirTodo");
app.UseAuthentication();
app.UseAuthorization();

// Inicializar DB
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
// ENDPOINTS
// ----------------------------------------------------

app.MapGet("/", () => "API de Tienda activa");

// GET es público
app.MapGet("/api/productos", async (AppDbContext db) =>
{
    return await db.Productos.ToListAsync();
});

// POST protegido con JWT
app.MapPost("/api/productos", async (AppDbContext db, Producto producto) =>
{
    db.Productos.Add(producto);
    await db.SaveChangesAsync();
    return Results.Created($"/api/productos/{producto.Id}", producto);
}).RequireAuthorization();

// PUT protegido con JWT
app.MapPut("/api/productos/{id}", async (AppDbContext db, int id, Producto productoActualizado) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    producto.Nombre = productoActualizado.Nombre;
    producto.Precio = productoActualizado.Precio;

    await db.SaveChangesAsync();
    return Results.Ok(producto);
}).RequireAuthorization();

// DELETE protegido con JWT
app.MapDelete("/api/productos/{id}", async (AppDbContext db, int id) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    db.Productos.Remove(producto);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

// Login con emisión de JWT Firmado Real
app.MapPost("/api/login", (UsuarioLogin login) =>
{
    if (login != null && 
        string.Equals(login.Usuario, "admin", StringComparison.OrdinalIgnoreCase) && 
        login.Password == "1234")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, login.Usuario) }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Results.Ok(new 
        { 
            token = tokenString, 
            usuario = login.Usuario,
            mensaje = "Inicio de sesión exitoso" 
        });
    }

    return Results.Unauthorized();
});

app.Run();

// DTO de inicio de sesión
public class UsuarioLogin
{
    public string Usuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}