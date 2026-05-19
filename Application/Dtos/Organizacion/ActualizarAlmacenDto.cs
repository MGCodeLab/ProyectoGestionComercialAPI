namespace Application.Dtos.Organizacion
{
    public class ActualizarAlmacenDto
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public int SucursalId { get; set; }
        public string? Descripcion { get; set; }
        public bool EsPrincipal { get; set; }
    }
}
