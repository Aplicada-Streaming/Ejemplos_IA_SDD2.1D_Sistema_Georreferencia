using GeoVial.Web.Components;
using GeoVial.Web.Servicios;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

// Sesión por circuito (mantiene el token del lado servidor) y cliente del contrato REST.
builder.Services.AddScoped<EstadoSesion>();
var urlApi = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7443/";
builder.Services.AddHttpClient<ClienteApi>(http => http.BaseAddress = new Uri(urlApi));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
