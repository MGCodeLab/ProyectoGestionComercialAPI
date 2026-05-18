namespace Application.Dtos.Catalogo
{
    public class MarcaProductoDto
    {
        public int Id { get; set; }
        public string PublicId { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? LogoUrl { get; set; }
        public bool Activo { get; set; }
    }
}
