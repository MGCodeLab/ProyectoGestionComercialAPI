namespace Application.Dtos.Producto
{
    public class CrearProductoDto
    {
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
    }
}
