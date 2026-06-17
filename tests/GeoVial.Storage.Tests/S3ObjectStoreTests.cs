using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using FluentAssertions;
using GeoVial.Storage.Abstractions;
using GeoVial.Storage.Providers.S3;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace GeoVial.Storage.Tests;

/// <summary>
/// Pruebas del adaptador S3 con un doble de <see cref="IAmazonS3"/> (NSubstitute): verifican la
/// traducción del contrato neutral a las operaciones del SDK, sin un servicio S3 real.
/// </summary>
public sealed class S3ObjectStoreTests
{
    private const string Bucket = "geovial";
    private readonly IAmazonS3 _s3 = Substitute.For<IAmazonS3>();

    private S3ObjectStore Crear() => new(_s3, new S3StorageOptions { Bucket = Bucket });

    private static AmazonS3Exception NoEncontrado() => new("no existe") { StatusCode = HttpStatusCode.NotFound };

    [Fact]
    public async Task Save_sube_al_bucket_con_clave_y_tipo()
    {
        var store = Crear();
        using var contenido = new MemoryStream(new byte[] { 1, 2, 3, 4 });

        var info = await store.SaveAsync("relevamientos/1/foto", contenido, "image/jpeg");

        info.Key.Should().Be("relevamientos/1/foto");
        info.SizeBytes.Should().Be(4);
        info.ContentType.Should().Be("image/jpeg");
        await _s3.Received(1).PutObjectAsync(
            Arg.Is<PutObjectRequest>(r => r.BucketName == Bucket && r.Key == "relevamientos/1/foto" && r.ContentType == "image/jpeg"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Get_devuelve_el_contenido()
    {
        _s3.GetObjectAsync(Bucket, "k", Arg.Any<CancellationToken>())
            .Returns(new GetObjectResponse { ResponseStream = new MemoryStream(new byte[] { 9, 9 }) });
        var store = Crear();

        await using var s = await store.GetAsync("k");
        using var ms = new MemoryStream();
        await s.CopyToAsync(ms);
        ms.ToArray().Should().Equal(9, 9);
    }

    [Fact]
    public async Task Get_inexistente_lanza_ObjectNotFound()
    {
        _s3.GetObjectAsync(Bucket, "missing", Arg.Any<CancellationToken>()).ThrowsAsync(NoEncontrado());
        var store = Crear();

        await store.Invoking(s => s.GetAsync("missing")).Should().ThrowAsync<ObjectNotFoundException>();
    }

    [Fact]
    public async Task Exists_segun_metadata()
    {
        _s3.GetObjectMetadataAsync(Bucket, "hay", Arg.Any<CancellationToken>()).Returns(new GetObjectMetadataResponse());
        _s3.GetObjectMetadataAsync(Bucket, "no", Arg.Any<CancellationToken>()).ThrowsAsync(NoEncontrado());
        var store = Crear();

        (await store.ExistsAsync("hay")).Should().BeTrue();
        (await store.ExistsAsync("no")).Should().BeFalse();
    }

    [Fact]
    public async Task Delete_existente_borra_y_devuelve_true()
    {
        _s3.GetObjectMetadataAsync(Bucket, "k", Arg.Any<CancellationToken>()).Returns(new GetObjectMetadataResponse());
        var store = Crear();

        (await store.DeleteAsync("k")).Should().BeTrue();
        await _s3.Received(1).DeleteObjectAsync(Bucket, "k", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_inexistente_no_borra_y_devuelve_false()
    {
        _s3.GetObjectMetadataAsync(Bucket, "k", Arg.Any<CancellationToken>()).ThrowsAsync(NoEncontrado());
        var store = Crear();

        (await store.DeleteAsync("k")).Should().BeFalse();
        await _s3.DidNotReceive().DeleteObjectAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task List_mapea_los_objetos()
    {
        _s3.ListObjectsV2Async(Arg.Any<ListObjectsV2Request>(), Arg.Any<CancellationToken>())
            .Returns(new ListObjectsV2Response
            {
                IsTruncated = false,
                S3Objects = new List<S3Object>
                {
                    new() { Key = "a", Size = 10, LastModified = DateTime.UtcNow },
                    new() { Key = "b", Size = 20, LastModified = DateTime.UtcNow },
                },
            });
        var store = Crear();

        var lista = new List<StoredObjectInfo>();
        await foreach (var info in store.ListAsync("a"))
        {
            lista.Add(info);
        }

        lista.Should().HaveCount(2);
        lista.Select(i => i.Key).Should().Contain(new[] { "a", "b" });
    }

    [Fact]
    public void Sin_bucket_no_se_puede_construir()
    {
        var accion = () => new S3ObjectStore(_s3, new S3StorageOptions { Bucket = null });

        accion.Should().Throw<ProviderNotConfiguredException>();
    }
}
