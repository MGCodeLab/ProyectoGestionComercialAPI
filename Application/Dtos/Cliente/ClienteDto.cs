namespace Application.Dtos.Cliente
{
    public class ClienteDto
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        public int TipoDocumentoId { get; set; }
        public required string NumeroDocumento { get; set; }
        public required string Nombres { get; set; }
        public required string ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}
