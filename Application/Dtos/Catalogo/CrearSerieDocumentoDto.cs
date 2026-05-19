namespace Application.Dtos.Catalogo
{
    public class CrearSerieDocumentoDto
    {
        public int TipoComprobanteId { get; set; }
        public int SucursalId { get; set; }
        public string Serie { get; set; }
        public int? NumeroMaximo { get; set; }
    }
}
