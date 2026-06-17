using System.Text;
using FluentAssertions;
using GeoVial.Storage.Abstractions;
using GeoVial.Storage.Providers.Local;
using Microsoft.Extensions.Options;

namespace GeoVial.Storage.Tests;

public sealed class LocalObjectStoreTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "geovial-storage-tests", Guid.NewGuid().ToString("N"));

    private LocalObjectStore CreateStore(long maxSizeBytes = 25L * 1024 * 1024)
        => new(Options.Create(new LocalStorageOptions { RootPath = _root, MaxSizeBytes = maxSizeBytes }));

    private static MemoryStream Bytes(string content) => new(Encoding.UTF8.GetBytes(content));

    [Fact]
    public async Task Guardar_y_recuperar_devuelve_el_mismo_contenido()
    {
        var store = CreateStore();

        var info = await store.SaveAsync("relevamientos/1/foto.jpg", Bytes("contenido-foto"), "image/jpeg");

        info.Key.Should().Be("relevamientos/1/foto.jpg");
        info.SizeBytes.Should().Be(Encoding.UTF8.GetByteCount("contenido-foto"));

        await using var recuperado = await store.GetAsync("relevamientos/1/foto.jpg");
        using var reader = new StreamReader(recuperado);
        (await reader.ReadToEndAsync()).Should().Be("contenido-foto");
    }

    [Fact]
    public async Task Recuperar_inexistente_lanza_ObjectNotFound()
    {
        var store = CreateStore();
        var act = () => store.GetAsync("no-existe.bin");
        await act.Should().ThrowAsync<ObjectNotFoundException>().Where(e => e.Code == "ARCHIVO_INEXISTENTE");
    }

    [Fact]
    public async Task Verificar_existencia_y_eliminar()
    {
        var store = CreateStore();
        await store.SaveAsync("a/b.txt", Bytes("x"), "text/plain");

        (await store.ExistsAsync("a/b.txt")).Should().BeTrue();
        (await store.DeleteAsync("a/b.txt")).Should().BeTrue();
        (await store.ExistsAsync("a/b.txt")).Should().BeFalse();
        (await store.DeleteAsync("a/b.txt")).Should().BeFalse();
    }

    [Fact]
    public async Task Listar_filtra_por_prefijo()
    {
        var store = CreateStore();
        await store.SaveAsync("rel/1/a.txt", Bytes("1"), "text/plain");
        await store.SaveAsync("rel/2/b.txt", Bytes("2"), "text/plain");
        await store.SaveAsync("otro/c.txt", Bytes("3"), "text/plain");

        var claves = new List<string>();
        await foreach (var obj in store.ListAsync("rel/"))
        {
            claves.Add(obj.Key);
        }

        claves.Should().BeEquivalentTo("rel/1/a.txt", "rel/2/b.txt");
    }

    [Fact]
    public async Task Guardar_que_supera_el_maximo_lanza_ObjectTooLarge()
    {
        var store = CreateStore(maxSizeBytes: 4);
        var act = () => store.SaveAsync("grande.bin", Bytes("demasiado-grande"), "application/octet-stream");
        await act.Should().ThrowAsync<ObjectTooLargeException>().Where(e => e.Code == "TAMANIO_EXCEDIDO");
    }

    [Theory]
    [InlineData("../escape.txt")]
    [InlineData("a/../../escape.txt")]
    public async Task Clave_con_traversal_lanza_InvalidObjectKey(string key)
    {
        var store = CreateStore();
        var act = () => store.SaveAsync(key, Bytes("x"), "text/plain");
        await act.Should().ThrowAsync<InvalidObjectKeyException>().Where(e => e.Code == "CLAVE_INVALIDA");
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }
}
