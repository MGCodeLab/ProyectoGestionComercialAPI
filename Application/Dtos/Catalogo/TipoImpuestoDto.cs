namespace Application.Dtos.Catalogo
{
    public class TipoImpuestoDto
    {
        public Guid PublicId { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public decimal Porcentaje { get; set; }
        public bool EsIncluido { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
