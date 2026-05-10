using Domain.Common;

namespace Domain.Catalogo;

/// <summary>
/// Catálogo de países disponibles en el sistema.
/// </summary>
public class Pais : AuditableEntity
{
    /// <summary>
    /// Nombre del país.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Código ISO 3166-1 alpha-2 (ej: PE, CL, AR, CO).
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Código de moneda ISO 4217 (ej: PEN, CLP, ARS, COP).
    /// </summary>
    public string CodigoMoneda { get; set; } = string.Empty;
}
