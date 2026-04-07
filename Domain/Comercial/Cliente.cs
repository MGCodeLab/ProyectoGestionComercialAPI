using Domain.Catalogo;
using Domain.Common;

namespace Domain.Comercial
{
    public class Cliente : AuditableEntity
    {
        public int TipoDocumentoId { get; set; }
        public virtual TipoDocumento TipoDocumento { get; set; } = null!;

        public required string NumeroDocumento { get; set; }

        public required string Nombres { get; set; }
        public required string ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }

        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }

        public string NombreCompleto { get; private set; } = null!;
    }
}
