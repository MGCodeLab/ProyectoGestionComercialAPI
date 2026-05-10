using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Catalogo;

/// <summary>DTO para actualizar un parámetro del sistema</summary>
public class ActualizarParametroSistemaDto
{
    /// <summary>Clave única del parámetro (ej: MONEDA_BASE, IGV_PORCENTAJE)</summary>
    [Required(ErrorMessage = "La clave del parámetro es requerida")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres en clave")]
    public required string Clave { get; set; }

    /// <summary>Valor del parámetro</summary>
    [Required(ErrorMessage = "El valor del parámetro es requerido")]
    [StringLength(500, ErrorMessage = "Máximo 500 caracteres en valor")]
    public required string Valor { get; set; }

    /// <summary>Tipo de dato del parámetro (STRING, INT, DECIMAL, BOOL)</summary>
    [Required(ErrorMessage = "El tipo de dato es requerido")]
    [StringLength(20, ErrorMessage = "Máximo 20 caracteres en tipo de dato")]
    public required string TipoDato { get; set; }

    /// <summary>Descripción opcional del parámetro</summary>
    [StringLength(500, ErrorMessage = "Máximo 500 caracteres en descripción")]
    public string? Descripcion { get; set; }
}
