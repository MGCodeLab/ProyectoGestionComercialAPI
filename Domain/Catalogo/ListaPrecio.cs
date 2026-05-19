using Domain.Common;

namespace Domain.Catalogo;

/// <summary>
/// Catálogo de listas de precios por moneda.
/// Abstracción simple sin detalles de productos (ListaPrecioDetalle diferido a Ventas).
/// </summary>
public class ListaPrecio : AuditableEntity
{
    /// <summary>
    /// Nombre descriptivo de la lista de precios.
    /// </summary>
    public required string Nombre { get; set; }

    /// <summary>
    /// Identificador de la moneda en que están expresados los precios.
    /// </summary>
    public int MonedaId { get; set; }

    /// <summary>
    /// Descripción de la lista de precios.
    /// </summary>
    public string? Descripcion { get; set; }

    /// <summary>
    /// Indica si esta es la lista de precios predeterminada del sistema.
    /// Solo una lista puede tener este valor en true.
    /// </summary>
    public bool EsDefault { get; set; } = false;

    /// <summary>
    /// Navegación a la moneda asociada.
    /// </summary>
    public virtual Moneda Moneda { get; set; } = null!;
}
