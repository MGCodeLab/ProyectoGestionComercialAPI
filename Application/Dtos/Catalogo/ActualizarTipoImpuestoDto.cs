namespace Application.Dtos.Catalogo
{
    public class ActualizarTipoImpuestoDto
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public decimal Porcentaje { get; set; }
        public bool EsIncluido { get; set; }
    }
}
