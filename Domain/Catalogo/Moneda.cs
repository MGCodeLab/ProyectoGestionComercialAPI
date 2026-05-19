using Domain.Common;

namespace Domain.Catalogo;

/// <summary>
/// Catálogo de monedas funcionales en el sistema.
/// </summary>
public class Moneda : AuditableEntity
{
    /// <summary>
    /// Nombre de la moneda (ej: Nuevo Sol, Peso Chileno).
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Símbolo de la moneda (ej: S/, $, €).
    /// </summary>
    public string Simbolo { get; set; } = string.Empty;

    /// <summary>
    /// Código ISO 4217 (ej: PEN, CLP, ARS, COP).
    /// </summary>
    public string CodigoISO { get; set; } = string.Empty;

    /// <summary>
    /// Indica si esta es la moneda base del sistema.
    /// Solo una moneda puede tener este valor en true.
    /// </summary>
    public bool EsMonedaBase { get; set; } = false;
}
