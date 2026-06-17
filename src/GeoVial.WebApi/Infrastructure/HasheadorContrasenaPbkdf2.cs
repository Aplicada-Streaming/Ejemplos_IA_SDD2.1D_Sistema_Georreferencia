using System.Security.Cryptography;
using GeoVial.WebApi.Application;

namespace GeoVial.WebApi.Infrastructure;

/// <summary>
/// Hasheo de contraseñas con PBKDF2 (SHA-256). Formato del hash almacenado:
/// "{iteraciones}.{saltBase64}.{hashBase64}". El salt es aleatorio por contraseña.
/// </summary>
public sealed class HasheadorContrasenaPbkdf2 : IHasheadorContrasena
{
    private const int Iteraciones = 100_000;
    private const int TamanoSalt = 16;
    private const int TamanoHash = 32;

    public string Hashear(string contrasena)
    {
        var salt = RandomNumberGenerator.GetBytes(TamanoSalt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(contrasena, salt, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);
        return $"{Iteraciones}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verificar(string contrasena, string hash)
    {
        var partes = hash.Split('.', 3);
        if (partes.Length != 3 || !int.TryParse(partes[0], out var iteraciones))
        {
            return false;
        }

        var salt = Convert.FromBase64String(partes[1]);
        var esperado = Convert.FromBase64String(partes[2]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(contrasena, salt, iteraciones, HashAlgorithmName.SHA256, esperado.Length);
        return CryptographicOperations.FixedTimeEquals(actual, esperado);
    }
}
