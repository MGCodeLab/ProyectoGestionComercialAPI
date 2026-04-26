using Domain.Common;

namespace Domain.Seguridad
{
    public class Permiso : AuditableEntity
    {
        public string Recurso { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
    }
}
