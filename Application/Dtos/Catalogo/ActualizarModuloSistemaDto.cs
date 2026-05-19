using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Catalogo;

/// <summary>DTO para actualizar un módulo del sistema</summary>
public class ActualizarModuloSistemaDto
{
    /// <summary>Nombre del módulo</summary>
    [Required(ErrorMessage = "El nombre del módulo es requerido")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres en nombre")]
    public required string Nombre { get; set; }

    /// <summary>Código único del módulo (ej: VENTAS, COMPRAS)</summary>
    [Required(ErrorMessage = "El código del módulo es requerido")]
    [StringLength(50, ErrorMessage = "Máximo 50 caracteres en código")]
    public required string Codigo { get; set; }

    /// <summary>Descripción opcional del módulo</summary>
    [StringLength(500, ErrorMessage = "Máximo 500 caracteres en descripción")]
    public string? Descripcion { get; set; }
}
