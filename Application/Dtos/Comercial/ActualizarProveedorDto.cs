using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Comercial;

public class ActualizarProveedorDto
{
    [Required(ErrorMessage = "El tipo de documento es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El tipo de documento debe ser válido")]
    public int TipoDocumentoId { get; set; }

    [Required(ErrorMessage = "El número de documento es requerido")]
    [StringLength(20, ErrorMessage = "El número de documento no puede exceder 20 caracteres")]
    public required string NumeroDocumento { get; set; }

    [Required(ErrorMessage = "La razón social es requerida")]
    [StringLength(200, ErrorMessage = "La razón social no puede exceder 200 caracteres")]
    public required string RazonSocial { get; set; }

    [StringLength(150, ErrorMessage = "El nombre comercial no puede exceder 150 caracteres")]
    public string? NombreComercial { get; set; }

    [Required(ErrorMessage = "El país es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El país debe ser válido")]
    public int PaisId { get; set; }

    [EmailAddress(ErrorMessage = "El correo debe ser válido")]
    [StringLength(150, ErrorMessage = "El correo no puede exceder 150 caracteres")]
    public string? Correo { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    public string? Telefono { get; set; }

    [StringLength(300, ErrorMessage = "La dirección no puede exceder 300 caracteres")]
    public string? Direccion { get; set; }
}
