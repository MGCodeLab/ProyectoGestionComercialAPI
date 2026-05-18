using Domain.Common;

namespace Domain.Catalogo
{
    public class MarcaProducto : AuditableEntity
    {
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? LogoUrl { get; set; }
    }
}
