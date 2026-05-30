namespace Application.Dtos.Organizacion
{
    public class SucursalDto
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public int EmpresaId { get; set; }
        public int PaisId { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public bool EsPrincipal { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public EmpresaSlimDto? Empresa { get; set; }
    }
}
