using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Cliente
{
    /// <summary>
    /// DTO para representar un cliente en respuestas de la API.
    /// Incluye datos personales, documento, contacto y auditoría.
    /// </summary>
    public class ClienteDto
    {
        /// <summary>Identificador único interno del cliente.</summary>
        public int Id { get; set; }

        /// <summary>Identificador público único (GUID) del cliente.</summary>
        public Guid PublicId { get; set; }

        /// <summary>Tipo de documento del cliente (FK a TipoDocumento).</summary>
        public int TipoDocumentoId { get; set; }

        /// <summary>Número de documento del cliente (DNI, RUC, etc.).</summary>
        [Required(ErrorMessage = "El número de documento es obligatorio")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "El documento debe tener entre 5 y 20 caracteres")]
        public required string NumeroDocumento { get; set; }

        /// <summary>Nombres del cliente.</summary>
        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Los nombres deben tener entre 2 y 100 caracteres")]
        public required string Nombres { get; set; }

        /// <summary>Apellido paterno del cliente.</summary>
        [Required(ErrorMessage = "El apellido paterno es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 100 caracteres")]
        public required string ApellidoPaterno { get; set; }

        /// <summary>Apellido materno del cliente (opcional).</summary>
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        public string? ApellidoMaterno { get; set; }

        /// <summary>Correo electrónico del cliente (opcional, único).</summary>
        [EmailAddress(ErrorMessage = "El correo debe ser una dirección de email válida")]
        [StringLength(150, ErrorMessage = "El correo no puede exceder 150 caracteres")]
        public string? Correo { get; set; }

        /// <summary>Teléfono de contacto del cliente (opcional).</summary>
        [Phone(ErrorMessage = "El teléfono debe ser un número válido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string? Telefono { get; set; }

        /// <summary>Dirección de envío del cliente (opcional).</summary>
        [StringLength(500, ErrorMessage = "La dirección no puede exceder 500 caracteres")]
        public string? Direccion { get; set; }

        /// <summary>Nombre completo computado (Nombres + ApellidoPaterno + ApellidoMaterno).</summary>
        [StringLength(300, ErrorMessage = "El nombre completo no puede exceder 300 caracteres")]
        public string NombreCompleto { get; set; } = string.Empty;

        /// <summary>Indica si el cliente está activo (true) o inactivo (false).</summary>
        public bool Activo { get; set; }

        /// <summary>Fecha de creación del registro (UTC).</summary>
        public DateTime FechaRegistro { get; set; }

        /// <summary>Fecha de última actualización del registro (UTC, nullable).</summary>
        public DateTime? FechaActualizacion { get; set; }
    }
}
