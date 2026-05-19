using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Producto
{
    /// <summary>
    /// DTO para actualizar un producto existente.
    /// Incluye todos los campos modificables del producto.
    /// </summary>
    public class ActualizarProductoDto
    {
        /// <summary>
        /// Nuevo nombre del producto.
        /// </summary>
        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 200 caracteres")]
        public required string Nombre { get; set; }

        /// <summary>
        /// Nueva descripción del producto (opcional).
        /// </summary>
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string? Descripcion { get; set; }

        /// <summary>
        /// Nuevo precio unitario del producto.
        /// </summary>
        [Range(typeof(decimal), "0.01", "999999.99", ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
        public decimal Precio { get; set; }

        /// <summary>
        /// Sprint 4: Identificador de la unidad de medida (opcional)
        /// </summary>
        public int? UnidadMedidaId { get; set; }

        /// <summary>
        /// Sprint 4: Identificador de la categoría del producto (opcional)
        /// </summary>
        public int? CategoriaProductoId { get; set; }

        /// <summary>
        /// Sprint 4: Identificador de la marca del producto (opcional)
        /// </summary>
        public int? MarcaProductoId { get; set; }
    }
}
