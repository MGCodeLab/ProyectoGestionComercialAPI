using Domain.Common;

namespace Domain.Catalogo;

/// <summary>
/// Términos y condiciones de pago para operaciones comerciales.
/// </summary>
public class CondicionPago : AuditableEntity
{
    /// <summary>
    /// Nombre de la condición de pago (ej: Contado, 15 Días, 30 Días).
    /// </summary>
    public required string Nombre { get; set; }

    /// <summary>
    /// Cantidad de días de crédito. 0 = Pago al contado.
    /// </summary>
    public int DiasCredito { get; set; } = 0;

    /// <summary>
    /// Descripción detallada de la condición de pago.
    /// </summary>
    public string? Descripcion { get; set; }
}
