using GeoVial.WebApi.Application;
using GeoVial.Storage.Abstractions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GeoVial.WebApi.Api;

/// <summary>
/// Traduce las excepciones de dominio/aplicación a respuestas problem+json (RFC 7807),
/// con un código estable en la extensión "codigo". Centraliza el manejo de errores del
/// contrato REST (ADR de manejo de errores).
/// </summary>
public sealed class ManejadorExcepciones(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var (status, codigo) = Mapear(exception);
        context.Response.StatusCode = status;

        var problema = new ProblemDetails
        {
            Status = status,
            Title = TituloPorEstado(status),
            Detail = exception.Message,
            Type = $"https://geovial/errores/{codigo}",
        };
        problema.Extensions["codigo"] = codigo;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = problema,
        });
    }

    private static (int Status, string Codigo) Mapear(Exception ex) => ex switch
    {
        CredencialesInvalidasException e => (StatusCodes.Status401Unauthorized, e.Codigo),
        OperacionNoAutorizadaException e => (StatusCodes.Status403Forbidden, e.Codigo),
        UsuarioYaExisteException e => (StatusCodes.Status409Conflict, e.Codigo),
        UsuarioNoEncontradoException e => (StatusCodes.Status404NotFound, e.Codigo),
        RelevamientoNoEncontradoException e => (StatusCodes.Status404NotFound, e.Codigo),
        TransicionEstadoInvalidaException e => (StatusCodes.Status409Conflict, e.Codigo),
        RelevamientoCerradoException e => (StatusCodes.Status409Conflict, e.Codigo),
        ErrorAplicacion e => (StatusCodes.Status400BadRequest, e.Codigo),
        ObjectNotFoundException e => (StatusCodes.Status404NotFound, e.Code),
        ObjectTooLargeException e => (StatusCodes.Status413PayloadTooLarge, e.Code),
        StorageException e => (StatusCodes.Status400BadRequest, e.Code),
        _ => (StatusCodes.Status500InternalServerError, "ERROR_INTERNO"),
    };

    private static string TituloPorEstado(int status) => status switch
    {
        401 => "No autenticado",
        403 => "Operación no autorizada",
        404 => "Recurso no encontrado",
        409 => "Conflicto",
        413 => "Contenido demasiado grande",
        400 => "Solicitud inválida",
        _ => "Error interno",
    };
}
