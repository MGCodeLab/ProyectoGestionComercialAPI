using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Catalogo;

public class ActualizarListaPrecioDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
    public required string Nombre { get; set; }

    [Required(ErrorMessage = "El ID de moneda es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de moneda debe ser válido")]
    public int MonedaId { get; set; }

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Descripcion { get; set; }

    public bool EsDefault { get; set; } = false;
}
