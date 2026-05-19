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

        public async Task<List<SerieDocumento>> ObtenerTodosAsync()
        {
            return await _context.SeriesDocumento
                .Include(x => x.TipoComprobante)
                .Include(x => x.Sucursal)
                .Where(x => x.Activo)
                .ToListAsync();
        }

        public async Task<SerieDocumento> ObtenerPorIdAsync(int id)
        {
            return await _context.SeriesDocumento
                .Include(x => x.TipoComprobante)
                .Include(x => x.Sucursal)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Crear(SerieDocumento serieDocumento)
        {
            _context.SeriesDocumento.Add(serieDocumento);
            await _context.SaveChangesAsync();
        }

        public async Task Actualizar(SerieDocumento serieDocumento)
        {
            _context.SeriesDocumento.Update(serieDocumento);
            await _context.SaveChangesAsync();
        }

        public async Task Eliminar(int id)
        {
            var serieDocumento = await ObtenerPorIdAsync(id);
            if (serieDocumento != null)
            {
                serieDocumento.Activo = false;
                await Actualizar(serieDocumento);
            }
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
                {
                    throw new InvalidOperationException(
                        $"Serie {serieDocumentoId} no encontrada o alcanzó límite máximo");
                }

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
