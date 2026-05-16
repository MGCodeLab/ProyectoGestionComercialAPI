namespace Application.Dtos.Organizacion
{
    public class ActualizarEmpresaDto
    {
        public string RazonSocial { get; set; }
        public string? NombreComercial { get; set; }
        public string NumeroDocumento { get; set; }
        public int TipoDocumentoId { get; set; }
        public int PaisId { get; set; }
        public int MonedaBaseId { get; set; }
        public string? DireccionFiscal { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? LogoUrl { get; set; }
    }
}
