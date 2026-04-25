using Domain.Common;

namespace Domain.Catalogo
{
    public class TipoDocumento : AuditableEntity
    {
        public required string Codigo { get; set; }
        public string? Descripcion { get; set; }
    }
}
