using Domain.Common;

namespace Domain.Catalogo
{
    public class TipoImpuesto : AuditableEntity
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public decimal Porcentaje { get; set; }
        public bool EsIncluido { get; set; }
    }
}
