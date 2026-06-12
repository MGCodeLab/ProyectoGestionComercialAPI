namespace Application.Dtos.Catalogo
{
    public class SerieDocumentoDto
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        public int TipoComprobanteId { get; set; }
        public int SucursalId { get; set; }
        public string Serie { get; set; }
        public int NumeroActual { get; set; }
        public int? NumeroMaximo { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}
