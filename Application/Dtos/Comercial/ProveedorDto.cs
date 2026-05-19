namespace Application.Dtos.Comercial;

public class ProveedorDto
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public int TipoDocumentoId { get; set; }
    public string NumeroDocumento { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? NombreComercial { get; set; }
    public int PaisId { get; set; }
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public string TipoDocumentoCodigo { get; set; } = string.Empty;
    public string PaisNombre { get; set; } = string.Empty;
}
