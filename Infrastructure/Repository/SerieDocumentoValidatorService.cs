using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class SerieDocumentoValidatorService
    {
        private readonly AppDbContext _context;

        public SerieDocumentoValidatorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SerieUnicaAsync(
            int tipoComprobanteId,
            int sucursalId,
            string serie,
            int? excludeId = null)
        {
            var existe = await _context.SeriesDocumento
                .Where(s => s.TipoComprobanteId == tipoComprobanteId
                    && s.SucursalId == sucursalId
                    && s.Serie == serie
                    && (excludeId == null || s.Id != excludeId))
                .AnyAsync();
            return !existe;
        }

        public async Task<bool> NumeroActualValido(int serieId, int numeroActual)
        {
            var serie = await _context.SeriesDocumento.FindAsync(serieId);
            if (serie?.NumeroMaximo == null) return true;
            return numeroActual < serie.NumeroMaximo;
        }
    }
}
