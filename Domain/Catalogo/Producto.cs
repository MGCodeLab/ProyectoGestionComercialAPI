using Domain.Common;

namespace Domain.Catalogo
{
    public class Producto : AuditableEntity
    {
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }

        // Sprint 4: New nullable foreign keys for enriched product
        public int? UnidadMedidaId { get; set; }
        public int? CategoriaProductoId { get; set; }
        public int? MarcaProductoId { get; set; }

        // Navigation properties
        public virtual UnidadMedida? UnidadMedida { get; set; }
        public virtual CategoriaProducto? CategoriaProducto { get; set; }
        public virtual MarcaProducto? MarcaProducto { get; set; }
    }
}
