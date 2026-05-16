using Domain.Common;
using Domain.Catalogo;

namespace Domain.Organizacion
{
    public class Sucursal : AuditableEntity
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public int EmpresaId { get; set; }
        public int PaisId { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public bool EsPrincipal { get; set; }

        public virtual Empresa? Empresa { get; set; }
        public virtual Pais? Pais { get; set; }
    }
}
