using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Catalogo;

public class CrearParametroSistemaDto
{
    [Required(ErrorMessage = "La clave es requerida")]
    [StringLength(100, ErrorMessage = "La clave no puede exceder 100 caracteres")]
    public required string Clave { get; set; }

    [Required(ErrorMessage = "El valor es requerido")]
    [StringLength(500, ErrorMessage = "El valor no puede exceder 500 caracteres")]
    public required string Valor { get; set; }

    [StringLength(20)]
    public string TipoDato { get; set; } = "STRING";

    [StringLength(500)]
    public string? Descripcion { get; set; }
}
