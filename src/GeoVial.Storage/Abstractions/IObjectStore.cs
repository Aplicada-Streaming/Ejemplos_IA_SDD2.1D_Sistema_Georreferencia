namespace GeoVial.Storage.Abstractions;

/// <summary>
/// Contrato de alojamiento de archivos transparente al consumidor.
/// La superficie pública no cambia según el proveedor activo (transparencia, RN-01):
/// el backend guarda, recupera, elimina, verifica y lista objetos sin conocer el
/// proveedor concreto (local, remoto u otro). Materializa los CU-01 a CU-05 de
/// 02_especificacion_funcional y el contrato de Abstractions de 05.
/// </summary>
public interface IObjectStore
{
    /// <summary>Guarda un objeto bajo una clave. Devuelve sus metadatos. (CU-01)</summary>
    Task<StoredObjectInfo> SaveAsync(string key, Stream content, string contentType, CancellationToken cancellationToken = default);

    /// <summary>Recupera el contenido de un objeto por su clave. (CU-02)</summary>
    /// <exception cref="ObjectNotFoundException">Si la clave no existe.</exception>
    Task<Stream> GetAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Elimina un objeto. Devuelve true si existía y se eliminó. (CU-03)</summary>
    Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Indica si existe un objeto con esa clave. (CU-04)</summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Lista los objetos cuya clave comienza con el prefijo dado. (CU-05)</summary>
    IAsyncEnumerable<StoredObjectInfo> ListAsync(string prefix, CancellationToken cancellationToken = default);
}
