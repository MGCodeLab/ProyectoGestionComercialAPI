namespace Application.Dtos.Catalogo
{
    public class CrearCategoriaProductoDto
    {
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int? CategoriaPadreId { get; set; }
    }
}
