using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Catalogo;

public class ActualizarCondicionPagoDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public required string Nombre { get; set; }

    [Range(0, 999, ErrorMessage = "Los días de crédito deben estar entre 0 y 999")]
    public int DiasCredito { get; set; } = 0;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Descripcion { get; set; }
}
