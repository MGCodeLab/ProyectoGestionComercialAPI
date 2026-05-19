using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Catalogo;

public class CrearModuloSistemaDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public required string Nombre { get; set; }

    [Required(ErrorMessage = "El código es requerido")]
    [StringLength(50, ErrorMessage = "El código no puede exceder 50 caracteres")]
    public required string Codigo { get; set; }

    [StringLength(500)]
    public string? Descripcion { get; set; }
}
