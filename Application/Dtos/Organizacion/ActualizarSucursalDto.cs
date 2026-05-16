namespace Application.Dtos.Organizacion
{
    public class ActualizarSucursalDto
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public int EmpresaId { get; set; }
        public int PaisId { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public bool EsPrincipal { get; set; }
    }
}
