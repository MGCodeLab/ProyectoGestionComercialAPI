namespace Application.Dtos.Organizacion
{
    public class AlmacenDto
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public int SucursalId { get; set; }
        public string? Descripcion { get; set; }
        public bool EsPrincipal { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public SucursalSlimDto? Sucursal { get; set; }
        public EmpresaSlimDto? Empresa { get; set; }
    }

    public class SucursalSlimDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class EmpresaSlimDto
    {
        public int Id { get; set; }
        public string RazonSocial { get; set; }
    }
}
