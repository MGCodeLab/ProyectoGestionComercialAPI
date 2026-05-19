using Domain.Common;

namespace Domain.Catalogo
{
    public class SerieDocumento : AuditableEntity
    {
        public int TipoComprobanteId { get; set; }
        public int SucursalId { get; set; }
        public string Serie { get; set; }
        public int NumeroActual { get; set; }
        public int? NumeroMaximo { get; set; }

        // Navigations
        public TipoComprobante TipoComprobante { get; set; }
        public Organizacion.Sucursal Sucursal { get; set; }
    }
}
