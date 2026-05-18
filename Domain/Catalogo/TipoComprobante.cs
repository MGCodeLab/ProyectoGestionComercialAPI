using Domain.Common;

namespace Domain.Catalogo
{
    public class TipoComprobante : AuditableEntity
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public bool AfectaInventario { get; set; }
        public bool AfectaContable { get; set; }

        // Navigation
        public ICollection<SerieDocumento> SeriesDocumento { get; set; } = new List<SerieDocumento>();
    }
}
