namespace Application.Dtos.Catalogo;

/// <summary>DTO de respuesta para TipoDocumento</summary>
public class TipoDocumentoDto
{
    /// <summary>Identificador único interno</summary>
    public int Id { get; set; }

    /// <summary>Identificador público (GUID)</summary>
    public Guid PublicId { get; set; }

    /// <summary>Código del tipo de documento (ej: 01, 03, NV)</summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>Descripción del tipo de documento</summary>
    public string? Descripcion { get; set; }

    /// <summary>Estado del registro</summary>
    public bool Activo { get; set; }

    /// <summary>Fecha de creación (UTC)</summary>
    public DateTime FechaRegistro { get; set; }

    /// <summary>Fecha de última actualización (UTC)</summary>
    public DateTime? FechaActualizacion { get; set; }
}
