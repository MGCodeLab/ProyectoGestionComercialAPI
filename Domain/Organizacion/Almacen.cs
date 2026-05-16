using Domain.Common;

namespace Domain.Organizacion
{
    public class Almacen : AuditableEntity
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public int SucursalId { get; set; }
        public string? Descripcion { get; set; }
        public bool EsPrincipal { get; set; }

        public virtual Sucursal? Sucursal { get; set; }
    }
}
