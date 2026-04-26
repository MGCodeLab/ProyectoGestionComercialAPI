namespace Application.Dtos.Cliente
{
    public class CrearClienteDto
    {
        public int TipoDocumentoId { get; set; }
        public required string NumeroDocumento { get; set; }
        public required string Nombres { get; set; }
        public required string ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
    }
}
