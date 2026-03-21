namespace Application.Dtos.Producto
{
    public class ActualizarProductoDto
    {
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public bool Activo { get; set; }
    }
}
