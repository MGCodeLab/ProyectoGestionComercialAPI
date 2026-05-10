using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Catalogo;

public class ActualizarUnidadMedidaDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public required string Nombre { get; set; }

    [Required(ErrorMessage = "El símbolo es requerido")]
    [StringLength(10, ErrorMessage = "El símbolo no puede exceder 10 caracteres")]
    public required string Simbolo { get; set; }

    [Required(ErrorMessage = "El código es requerido")]
    [StringLength(10, ErrorMessage = "El código no puede exceder 10 caracteres")]
    public required string Codigo { get; set; }
}
