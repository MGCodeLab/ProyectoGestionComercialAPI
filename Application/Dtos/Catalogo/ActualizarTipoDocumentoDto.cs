using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Catalogo;

/// <summary>DTO para actualizar un tipo de documento</summary>
public class ActualizarTipoDocumentoDto
{
    /// <summary>Código del tipo de documento (ej: 01, 03, NV)</summary>
    [Required(ErrorMessage = "El código es requerido")]
    [StringLength(5, ErrorMessage = "El código no puede exceder 5 caracteres")]
    public required string Codigo { get; set; }

    /// <summary>Descripción del tipo de documento</summary>
    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Descripcion { get; set; }
}
