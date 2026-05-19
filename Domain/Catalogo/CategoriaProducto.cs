using Domain.Common;
using System.Collections.Generic;

namespace Domain.Catalogo
{
    public class CategoriaProducto : AuditableEntity
    {
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int? CategoriaPadreId { get; set; }

        // Navigation properties for hierarchical tree
        public virtual CategoriaProducto? CategoriaPadre { get; set; }
        public virtual ICollection<CategoriaProducto> Subcategorias { get; set; } = new List<CategoriaProducto>();
    }
}
