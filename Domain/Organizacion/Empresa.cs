using Domain.Common;
using Domain.Catalogo;

namespace Domain.Organizacion
{
    public class Empresa : AuditableEntity
    {
        public string RazonSocial { get; set; }
        public string? NombreComercial { get; set; }
        public string NumeroDocumento { get; set; }
        public int TipoDocumentoId { get; set; }
        public int PaisId { get; set; }
        public int MonedaBaseId { get; set; }
        public string? DireccionFiscal { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? LogoUrl { get; set; }

        public virtual TipoDocumento? TipoDocumento { get; set; }
        public virtual Pais? Pais { get; set; }
        public virtual Moneda? MonedaBase { get; set; }
    }
}
