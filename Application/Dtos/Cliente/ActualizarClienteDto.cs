using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Cliente
{
    /// <summary>
    /// DTO para actualizar un cliente existente.
    /// Contiene los datos que pueden ser modificados de un cliente.
    /// </summary>
    public class ActualizarClienteDto
    {
        /// <summary>Nuevo tipo de documento del cliente (FK a TipoDocumento).</summary>
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de documento válido")]
        public int TipoDocumentoId { get; set; }

        /// <summary>Nuevo número de documento del cliente.</summary>
        [Required(ErrorMessage = "El número de documento es obligatorio")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "El documento debe tener entre 5 y 20 caracteres")]
        public required string NumeroDocumento { get; set; }

        /// <summary>Nuevos nombres del cliente.</summary>
        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Los nombres deben tener entre 2 y 100 caracteres")]
        public required string Nombres { get; set; }

        /// <summary>Nuevo apellido paterno del cliente.</summary>
        [Required(ErrorMessage = "El apellido paterno es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 100 caracteres")]
        public required string ApellidoPaterno { get; set; }

        /// <summary>Nuevo apellido materno del cliente (opcional).</summary>
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        public string? ApellidoMaterno { get; set; }

        /// <summary>Nuevo correo electrónico del cliente (opcional, único).</summary>
        [EmailAddress(ErrorMessage = "El correo debe ser una dirección de email válida")]
        [StringLength(150, ErrorMessage = "El correo no puede exceder 150 caracteres")]
        public string? Correo { get; set; }

        /// <summary>Nuevo teléfono de contacto del cliente (opcional).</summary>
        [Phone(ErrorMessage = "El teléfono debe ser un número válido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string? Telefono { get; set; }

        /// <summary>Nueva dirección de envío del cliente (opcional).</summary>
        [StringLength(500, ErrorMessage = "La dirección no puede exceder 500 caracteres")]
        public string? Direccion { get; set; }
    }
}
