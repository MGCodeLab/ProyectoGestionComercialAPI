using Domain.Common;

namespace Domain.Configuracion;

/// <summary>
/// Parámetros de configuración del sistema.
/// Almacena claves y valores configurables.
/// </summary>
public class ParametroSistema : AuditableEntity
{
    /// <summary>
    /// Clave del parámetro (ej: MONEDA_BASE, IGV_PORCENTAJE).
    /// </summary>
    public string Clave { get; set; } = string.Empty;

    /// <summary>
    /// Valor del parámetro.
    /// </summary>
    public string Valor { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de dato del valor (STRING, INT, DECIMAL, BOOL).
    /// </summary>
    public string TipoDato { get; set; } = "STRING";

    /// <summary>
    /// Descripción del parámetro y su uso.
    /// </summary>
    public string? Descripcion { get; set; }
}
