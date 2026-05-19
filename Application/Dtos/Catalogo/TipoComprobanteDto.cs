namespace Application.Dtos.Catalogo
{
    public class TipoComprobanteDto
    {
        public Guid PublicId { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public bool AfectaInventario { get; set; }
        public bool AfectaContable { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
