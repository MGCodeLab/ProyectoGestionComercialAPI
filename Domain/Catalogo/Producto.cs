using Domain.Common;

namespace Domain.Catalogo
{
    public class Producto : AuditableEntity
    {
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
    }
}
