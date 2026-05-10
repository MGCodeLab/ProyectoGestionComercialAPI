namespace Application.Dtos.Catalogo;

/// <summary>
/// DTO de respuesta para un país.
/// </summary>
public class PaisDto
{
    /// <summary>
    /// Identificador interno.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador público (GUID).
    /// </summary>
    public Guid PublicId { get; set; }

    /// <summary>
    /// Nombre del país.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Código ISO 3166-1 alpha-2.
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Código de moneda ISO 4217.
    /// </summary>
    public string CodigoMoneda { get; set; } = string.Empty;

    /// <summary>
    /// Indica si el país está activo.
    /// </summary>
    public bool Activo { get; set; }

    /// <summary>
    /// Fecha de registro.
    /// </summary>
    public DateTime FechaRegistro { get; set; }

    /// <summary>
    /// Fecha de última actualización.
    /// </summary>
    public DateTime? FechaActualizacion { get; set; }
}
