namespace Application.Dtos.Catalogo
{
    public class CategoriaProductoDto
    {
        public int Id { get; set; }
        public string PublicId { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int? CategoriaPadreId { get; set; }
        public bool Activo { get; set; }
        public List<CategoriaProductoDto>? Subcategorias { get; set; }
    }
}
