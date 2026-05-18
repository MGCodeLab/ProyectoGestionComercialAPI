namespace Application.Dtos.Catalogo
{
    public class ActualizarTipoComprobanteDto
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public bool AfectaInventario { get; set; }
        public bool AfectaContable { get; set; }
    }
}
