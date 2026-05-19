using Domain.Catalogo;
using Domain.Common;

namespace Domain.Comercial;

/// <summary>
/// Maestro de proveedores. Patrón idéntico a Cliente.
/// </summary>
public class Proveedor : AuditableEntity
{
    /// <summary>
    /// Tipo de documento de identificación del proveedor.
    /// </summary>
    public int TipoDocumentoId { get; set; }

    /// <summary>
    /// Número de documento del proveedor (RUC, DNI, Passport, etc.).
    /// </summary>
    public required string NumeroDocumento { get; set; }

    /// <summary>
    /// Razón social del proveedor.
    /// </summary>
    public required string RazonSocial { get; set; }

    /// <summary>
    /// Nombre comercial del proveedor (opcional, puede ser igual a RazonSocial).
    /// </summary>
    public string? NombreComercial { get; set; }

    /// <summary>
    /// País donde está registrado el proveedor.
    /// </summary>
    public int PaisId { get; set; }

    /// <summary>
    /// Correo de contacto (único, permite múltiples NULL).
    /// </summary>
    public string? Correo { get; set; }

    /// <summary>
    /// Teléfono de contacto.
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// Dirección del proveedor.
    /// </summary>
    public string? Direccion { get; set; }

    /// <summary>
    /// Navegación a tipo de documento.
    /// </summary>
    public virtual TipoDocumento TipoDocumento { get; set; } = null!;

    /// <summary>
    /// Navegación a país.
    /// </summary>
    public virtual Pais Pais { get; set; } = null!;
}
