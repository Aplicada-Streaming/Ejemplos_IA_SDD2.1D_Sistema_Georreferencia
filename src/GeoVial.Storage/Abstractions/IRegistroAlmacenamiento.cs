namespace GeoVial.Storage.Abstractions;

/// <summary>Resultado de validar un proveedor de almacenamiento sin activarlo (CU-17 5.B).</summary>
public sealed record ResultadoValidacionAlmacen(bool Valido, string? Detalle);

/// <summary>
/// Registro de los proveedores de almacenamiento disponibles y del destino activo. Permite al
/// nivel raíz consultar, validar y conmutar el destino (CU-17), de forma transparente para los
/// consumidores de <see cref="IObjectStore"/>. La transparencia del proveedor (RN-01 de
/// geovial-storage) se mantiene: los demás roles operan siempre contra <see cref="IObjectStore"/>.
/// </summary>
public interface IRegistroAlmacenamiento
{
    /// <summary>Identificador del proveedor activo (destino de las escrituras nuevas).</summary>
    string Activo { get; }

    /// <summary>Identificadores de los proveedores disponibles para activar.</summary>
    IReadOnlyCollection<string> Disponibles { get; }

    /// <summary>Establece el proveedor activo. Lanza si el proveedor no está disponible.</summary>
    void Activar(string proveedor);

    /// <summary>Valida que un proveedor pueda alojar y recuperar archivos, sin activarlo (CU-17 5.B).</summary>
    Task<ResultadoValidacionAlmacen> ValidarAsync(string proveedor, CancellationToken cancellationToken = default);
}
