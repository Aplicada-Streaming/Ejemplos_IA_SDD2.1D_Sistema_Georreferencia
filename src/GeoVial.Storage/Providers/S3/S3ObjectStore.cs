using System.Net;
using System.Runtime.CompilerServices;
using Amazon.S3;
using Amazon.S3.Model;
using GeoVial.Storage.Abstractions;

namespace GeoVial.Storage.Providers.S3;

/// <summary>
/// Adaptador de <see cref="IObjectStore"/> sobre un servicio de objetos compatible con S3
/// (AWS S3, MinIO, Cloudflare R2, etc.) a través de <see cref="IAmazonS3"/>. Es transparente al
/// consumidor (RN-01): la API del backend no cambia según el destino activo. El binario vive en el
/// bucket; aquí se traduce el contrato neutral de la librería a las operaciones del SDK.
/// </summary>
public sealed class S3ObjectStore : IObjectStore
{
    private readonly IAmazonS3 _cliente;
    private readonly string _bucket;

    public S3ObjectStore(IAmazonS3 cliente, S3StorageOptions opciones)
    {
        _cliente = cliente;
        _bucket = string.IsNullOrWhiteSpace(opciones?.Bucket)
            ? throw new ProviderNotConfiguredException("Falta configurar el bucket de S3 (Storage:S3:Bucket).")
            : opciones!.Bucket!;
    }

    public async Task<StoredObjectInfo> SaveAsync(string key, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidObjectKeyException(key);
        }

        // Se copia a memoria para conocer el tamaño y habilitar reintentos del SDK.
        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);
        buffer.Position = 0;

        var request = new PutObjectRequest
        {
            BucketName = _bucket,
            Key = key,
            InputStream = buffer,
            ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType,
            AutoCloseStream = false,
        };

        await _cliente.PutObjectAsync(request, cancellationToken);
        return new StoredObjectInfo(key, buffer.Length, request.ContentType, DateTimeOffset.UtcNow);
    }

    public async Task<Stream> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var respuesta = await _cliente.GetObjectAsync(_bucket, key, cancellationToken);
            return respuesta.ResponseStream;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            throw new ObjectNotFoundException(key);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cliente.GetObjectMetadataAsync(_bucket, key, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        // El contrato devuelve si el objeto existía; S3 no distingue al borrar uno ausente.
        if (!await ExistsAsync(key, cancellationToken))
        {
            return false;
        }

        await _cliente.DeleteObjectAsync(_bucket, key, cancellationToken);
        return true;
    }

    public async IAsyncEnumerable<StoredObjectInfo> ListAsync(string prefix, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? token = null;
        do
        {
            var respuesta = await _cliente.ListObjectsV2Async(
                new ListObjectsV2Request { BucketName = _bucket, Prefix = prefix ?? string.Empty, ContinuationToken = token },
                cancellationToken);

            foreach (var obj in respuesta.S3Objects)
            {
                var fecha = new DateTimeOffset(DateTime.SpecifyKind(obj.LastModified, DateTimeKind.Utc));
                yield return new StoredObjectInfo(obj.Key, obj.Size, "application/octet-stream", fecha);
            }

            token = respuesta.IsTruncated ? respuesta.NextContinuationToken : null;
        }
        while (token is not null);
    }
}
