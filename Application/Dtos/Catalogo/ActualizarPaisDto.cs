using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Catalogo;

/// <summary>
/// DTO para actualizar un país existente.
/// </summary>
public class ActualizarPaisDto
{
    /// <summary>
    /// Nombre del país.
    /// </summary>
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public required string Nombre { get; set; }

    /// <summary>
    /// Código ISO 3166-1 alpha-2.
    /// </summary>
    [Required(ErrorMessage = "El código ISO es requerido")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "El código debe ser de 2 caracteres")]
    public required string Codigo { get; set; }

    /// <summary>
    /// Código de moneda ISO 4217.
    /// </summary>
    [Required(ErrorMessage = "El código de moneda es requerido")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "El código de moneda debe ser de 3 caracteres")]
    public required string CodigoMoneda { get; set; }
}
