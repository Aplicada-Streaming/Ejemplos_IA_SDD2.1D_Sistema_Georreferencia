using GeoVial.Storage.DependencyInjection;
using GeoVial.WebApi.Api;
using GeoVial.WebApi.Application;
using GeoVial.WebApi.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// --- Controllers, OpenAPI y problem+json ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ManejadorExcepciones>();

// --- Persistencia (almacén relacional; proveedor por configuración) ---
var proveedor = builder.Configuration["Persistencia:Proveedor"] ?? "sqlite";
var cadena = builder.Configuration.GetConnectionString("GeoVial") ?? "Data Source=geovial.db";
builder.Services.AddDbContext<GeoVialDbContext>(opciones =>
{
    if (proveedor.Equals("sqlserver", StringComparison.OrdinalIgnoreCase))
    {
        opciones.UseSqlServer(cadena);
    }
    else
    {
        opciones.UseSqlite(cadena);
    }
});

// --- Almacenamiento de archivos (librería GeoVial.Storage) ---
builder.Services.AddGeoVialStorage(builder.Configuration);

// --- Servicios de aplicación e infraestructura ---
builder.Services.Configure<OpcionesToken>(builder.Configuration.GetSection(OpcionesToken.SectionName));
builder.Services.AddSingleton<IHasheadorContrasena, HasheadorContrasenaPbkdf2>();
builder.Services.AddSingleton<IEmisorTokens, EmisorTokensJwt>();
builder.Services.AddScoped<IServicioUsuarios, ServicioUsuarios>();
builder.Services.AddScoped<IServicioAutenticacion, ServicioAutenticacion>();
builder.Services.AddScoped<IServicioRelevamientos, ServicioRelevamientos>();
builder.Services.AddScoped<IServicioFotos, ServicioFotos>();
builder.Services.AddScoped<IServicioSincronizacion, ServicioSincronizacion>();

// --- Autenticación por token bearer ---
// La validación se configura desde el mismo OpcionesToken que usa el emisor (IOptions),
// para que la clave de firma coincida con la configuración final del entorno.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfiguradorJwtBearer>();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Inicialización del almacén y seed del usuario raíz.
await SeedInicial.InicializarAsync(app.Services);

app.Run();

/// <summary>Punto de entrada expuesto para las pruebas de integración (WebApplicationFactory).</summary>
public partial class Program;
