using FluentAssertions;
using GeoVial.Storage.Abstractions;
using GeoVial.Storage.Providers;
using GeoVial.Storage.Providers.Memory;

namespace GeoVial.Storage.Tests;

public sealed class RouterObjectStoreTests
{
    private static RouterObjectStore Crear(out MemoryObjectStore a, out MemoryObjectStore b, string activo = "a")
    {
        a = new MemoryObjectStore();
        b = new MemoryObjectStore();
        var proveedores = new Dictionary<string, IObjectStore>(StringComparer.OrdinalIgnoreCase) { ["a"] = a, ["b"] = b };
        return new RouterObjectStore(proveedores, activo);
    }

    private static MemoryStream Bytes(params byte[] datos) => new(datos);

    [Fact]
    public async Task Escribe_en_el_activo_y_lee_lo_anterior_tras_conmutar()
    {
        var router = Crear(out var a, out var b);

        // Se aloja con "a" activo.
        await router.SaveAsync("k1", Bytes(1, 2, 3), "application/octet-stream");
        (await a.ExistsAsync("k1")).Should().BeTrue();

        // CU-17: el raíz cambia el destino activo.
        router.Activar("b");
        router.Activo.Should().Be("b");

        // CU-17 5.A: lo alojado antes del cambio sigue accesible (fallback de lectura).
        await using (var leido = await router.GetAsync("k1"))
        {
            using var ms = new MemoryStream();
            await leido.CopyToAsync(ms);
            ms.ToArray().Should().Equal(1, 2, 3);
        }

        // Las escrituras nuevas van al destino activo.
        await router.SaveAsync("k2", Bytes(9), "application/octet-stream");
        (await b.ExistsAsync("k2")).Should().BeTrue();
        (await a.ExistsAsync("k2")).Should().BeFalse();
    }

    [Fact]
    public async Task Validar_proveedor_disponible_e_inexistente()
    {
        var router = Crear(out _, out _);

        (await router.ValidarAsync("b")).Valido.Should().BeTrue();   // CU-17 5.B: valida sin activar
        router.Activo.Should().Be("a");                              // no cambió el activo

        (await router.ValidarAsync("inexistente")).Valido.Should().BeFalse();
    }

    [Fact]
    public void Activar_proveedor_inexistente_falla()
    {
        var router = Crear(out _, out _);

        var accion = () => router.Activar("inexistente");

        accion.Should().Throw<ProviderNotConfiguredException>();
    }

    [Fact]
    public void Activo_inicial_inexistente_falla_al_construir()
    {
        var proveedores = new Dictionary<string, IObjectStore>(StringComparer.OrdinalIgnoreCase) { ["a"] = new MemoryObjectStore() };

        var accion = () => new RouterObjectStore(proveedores, "no-existe");

        accion.Should().Throw<ProviderNotConfiguredException>();
    }
}
