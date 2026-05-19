namespace Application.Interfaces;

public interface IProveedorValidatorService
{
    Task<bool> ProveedorUnicoAsync(int tipoDocumentoId, string numeroDocumento, int? excludeId = null);
    Task<bool> CorreoUnicoAsync(string? correo, int? excludeId = null);
    Task<bool> TipoDocumentoExisteAsync(int tipoDocumentoId);
    Task<bool> PaisExisteAsync(int paisId);
}
