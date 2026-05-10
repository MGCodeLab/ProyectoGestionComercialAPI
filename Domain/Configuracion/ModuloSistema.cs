using Domain.Common;

namespace Domain.Configuracion;

/// <summary>
/// Módulos del sistema con flags de activación.
/// Controla qué módulos están habilitados en el sistema.
/// El campo Activo (heredado de AuditableEntity) indica el estado.
/// </summary>
public class ModuloSistema : AuditableEntity
{
    /// <summary>
    /// Nombre del módulo (ej: VENTAS, COMPRAS, INVENTARIO).
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Código único del módulo usado internamente.
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del módulo y su propósito.
    /// </summary>
    public string? Descripcion { get; set; }
}
