using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Producto
{
    /// <summary>
    /// DTO para representar un producto en respuestas de la API.
    /// Incluye datos de auditoría como estado activo.
    /// </summary>
    public class ProductoDto
    {
        /// <summary>
        /// Identificador único interno del producto.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 200 caracteres")]
        public required string Nombre { get; set; }

        /// <summary>
        /// Descripción detallada del producto (opcional).
        /// </summary>
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string? Descripcion { get; set; }

        /// <summary>
        /// Precio unitario del producto.
        /// </summary>
        [Range(typeof(decimal), "0.01", "999999.99", ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
        public decimal Precio { get; set; }

        /// <summary>
        /// Indica si el producto está activo (true) o inactivo (false).
        /// </summary>
        public bool Activo { get; set; }
    }
}
