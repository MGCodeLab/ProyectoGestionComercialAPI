namespace Application.Dtos.Catalogo;

/// <summary>DTO de respuesta para Moneda</summary>
public class MonedaDto
{
    /// <summary>Identificador único interno</summary>
    public int Id { get; set; }

    /// <summary>Identificador público (GUID)</summary>
    public Guid PublicId { get; set; }

    /// <summary>Nombre de la moneda (ej: Dólar Americano)</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Símbolo de la moneda (ej: $, €, S/)</summary>
    public string Simbolo { get; set; } = string.Empty;

    /// <summary>Código ISO 4217 (ej: USD, PEN, EUR)</summary>
    public string CodigoISO { get; set; } = string.Empty;

    /// <summary>Indica si es la moneda base del sistema</summary>
    public bool EsMonedaBase { get; set; }

    /// <summary>Estado del registro</summary>
    public bool Activo { get; set; }

    /// <summary>Fecha de creación (UTC)</summary>
    public DateTime FechaRegistro { get; set; }

    /// <summary>Fecha de última actualización (UTC)</summary>
    public DateTime? FechaActualizacion { get; set; }
}
