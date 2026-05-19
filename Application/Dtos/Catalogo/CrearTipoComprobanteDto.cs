namespace Application.Dtos.Catalogo
{
    public class CrearTipoComprobanteDto
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public bool AfectaInventario { get; set; }
        public bool AfectaContable { get; set; }
    }
}
