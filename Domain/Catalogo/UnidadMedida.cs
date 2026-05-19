using Domain.Common;

namespace Domain.Catalogo;

/// <summary>
/// Catálogo de unidades de medida para productos.
/// </summary>
public class UnidadMedida : AuditableEntity
{
    /// <summary>
    /// Nombre de la unidad de medida (ej: Kilogramo, Litro, Unidad).
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Símbolo o abreviatura (ej: KG, LT, UND, MTR).
    /// </summary>
    public string Simbolo { get; set; } = string.Empty;

    /// <summary>
    /// Código normalizado SUNAT (ej: KGM, LTR, NIU, ZZ).
    /// </summary>
    public string Codigo { get; set; } = string.Empty;
}
