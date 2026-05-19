using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class ProveedorValidatorService : IProveedorValidatorService
{
    private readonly AppDbContext _context;

    public ProveedorValidatorService(AppDbContext context) => _context = context;

    public async Task<bool> ProveedorUnicoAsync(int tipoDocumentoId, string numeroDocumento, int? excludeId = null)
    {
        var existe = await _context.Proveedores
            .AnyAsync(p => p.TipoDocumentoId == tipoDocumentoId
                && p.NumeroDocumento == numeroDocumento
                && (excludeId == null || p.Id != excludeId));
        return !existe;
    }

    public async Task<bool> CorreoUnicoAsync(string? correo, int? excludeId = null)
    {
        if (string.IsNullOrEmpty(correo))
            return true;

        var existe = await _context.Proveedores
            .AnyAsync(p => p.Correo == correo && (excludeId == null || p.Id != excludeId));
        return !existe;
    }

    public async Task<bool> TipoDocumentoExisteAsync(int tipoDocumentoId)
    {
        return await _context.TipoDocumentos
            .AnyAsync(td => td.Id == tipoDocumentoId && td.Activo);
    }

    public async Task<bool> PaisExisteAsync(int paisId)
    {
        return await _context.Paises
            .AnyAsync(p => p.Id == paisId && p.Activo);
    }
}
