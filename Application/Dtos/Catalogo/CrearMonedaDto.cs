using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Catalogo;

/// <summary>DTO para crear una moneda</summary>
public class CrearMonedaDto
{
    /// <summary>Nombre de la moneda (ej: Dólar Americano)</summary>
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public required string Nombre { get; set; }

    /// <summary>Símbolo de la moneda (ej: $, €, S/)</summary>
    [Required(ErrorMessage = "El símbolo es requerido")]
    [StringLength(5, ErrorMessage = "El símbolo no puede exceder 5 caracteres")]
    public required string Simbolo { get; set; }

    /// <summary>Código ISO 4217 (ej: USD, PEN, EUR)</summary>
    [Required(ErrorMessage = "El código ISO es requerido")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "El código ISO debe ser exactamente 3 caracteres")]
    public required string CodigoISO { get; set; }

    /// <summary>Indica si es la moneda base del sistema</summary>
    public bool EsMonedaBase { get; set; }
}
