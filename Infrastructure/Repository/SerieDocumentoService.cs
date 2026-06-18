using Application.Interfaces;
using Domain.Catalogo;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository
{
    public class SerieDocumentoService : ISerieDocumentoService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SerieDocumentoService> _logger;

        public SerieDocumentoService(AppDbContext context, ILogger<SerieDocumentoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<SerieDocumento>> ObtenerTodos(CancellationToken token)
            => await _context.SeriesDocumento
                .Include(x => x.TipoComprobante)
                .Include(x => x.Sucursal)
                .ToListAsync(token);

        public async Task<SerieDocumento?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token)
            => isAsTracking
                ? await _context.SeriesDocumento
                    .Include(x => x.TipoComprobante)
                    .Include(x => x.Sucursal)
                    .FirstOrDefaultAsync(x => x.Id == id, token)
                : await _context.SeriesDocumento
                    .AsNoTracking()
                    .Include(x => x.TipoComprobante)
                    .Include(x => x.Sucursal)
                    .FirstOrDefaultAsync(x => x.Id == id, token);

        public async Task<int> Crear(SerieDocumento entity, CancellationToken token)
        {
            _context.SeriesDocumento.Add(entity);
            await _context.SaveChangesAsync(token);
            return entity.Id;
        }

        public async Task Actualizar(CancellationToken token)
            => await _context.SaveChangesAsync(token);

        public async Task Eliminar(SerieDocumento entity, CancellationToken token)
        {
            _context.SeriesDocumento.Remove(entity);
            await _context.SaveChangesAsync(token);
        }

        public async Task<int> ObtenerProximoNumeroAsync(int serieDocumentoId, CancellationToken ct = default)
        {
            using var transaction = await _context.Database
                .BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);

            try
            {
                var resultado = await _context.SeriesDocumento
                    .FromSqlInterpolated($@"
                        UPDATE catalogo.SeriesDocumento WITH (ROWLOCK, UPDLOCK)
                        SET NumeroActual = NumeroActual + 1
                        WHERE Id = {serieDocumentoId}
                            AND (NumeroMaximo IS NULL OR NumeroActual < NumeroMaximo)

                        SELECT * FROM catalogo.SeriesDocumento
                        WHERE Id = {serieDocumentoId}
                    ")
                    .ToListAsync(ct);

                var serie = resultado.FirstOrDefault();

                if (serie == null)
                    throw new InvalidOperationException(
                        $"Serie {serieDocumentoId} no encontrada o alcanzó límite máximo");

                await transaction.CommitAsync(ct);

                _logger.LogInformation(
                    "SerieDocumento {SerieId}: Próximo número asignado: {Numero}",
                    serieDocumentoId, serie.NumeroActual);

                return serie.NumeroActual;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);
                _logger.LogError(ex,
                    "Error al obtener próximo número para SerieDocumento {SerieId}",
                    serieDocumentoId);
                throw;
            }
        }
    }
}
