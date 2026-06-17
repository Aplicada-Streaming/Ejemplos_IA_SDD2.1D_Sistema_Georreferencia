namespace GeoVial.WebApi.Domain;

/// <summary>
/// Roles de la jerarquía de usuarios, de mayor a menor alcance (SOLUTION-INTAKE §2).
/// El valor numérico representa el nivel: menor número = mayor alcance.
/// </summary>
public enum Rol
{
    Raiz = 0,
    JefeGeneral = 1,
    JefeDeArea = 2,
    AgenteDeCampo = 3,
}

/// <summary>
/// Reglas de la jerarquía de administración (RN-01): cada nivel administra altas y
/// bajas del nivel inmediatamente inferior, y nunca de su propio nivel o superior.
/// </summary>
public static class Jerarquia
{
    /// <summary>Rol que un administrador de <paramref name="rol"/> puede dar de alta/baja.</summary>
    public static Rol? RolQueAdministra(this Rol rol) => rol switch
    {
        Rol.Raiz => Rol.JefeGeneral,
        Rol.JefeGeneral => Rol.JefeDeArea,
        Rol.JefeDeArea => Rol.AgenteDeCampo,
        _ => null,
    };

    /// <summary>Indica si <paramref name="administrador"/> puede administrar usuarios con <paramref name="rolDestino"/>.</summary>
    public static bool PuedeAdministrar(this Rol administrador, Rol rolDestino)
        => administrador.RolQueAdministra() == rolDestino;
}
