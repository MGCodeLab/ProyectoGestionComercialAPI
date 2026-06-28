using MediatR;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Features.Organizacion.Almacen.Eliminar
{
    public class EliminarAlmacenHandler : IRequestHandler<EliminarAlmacenCommand, int>
    {
        private readonly IAlmacenService _service;
        private readonly ILogger<EliminarAlmacenHandler> _logger;

        public EliminarAlmacenHandler(IAlmacenService service, ILogger<EliminarAlmacenHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<int> Handle(EliminarAlmacenCommand request, CancellationToken ct)
        {
            var almacen = await _service.ObtenerPorId(request.Id, true, ct);
            if (almacen == null)
                throw new KeyNotFoundException($"Almacén con Id {request.Id} no encontrado");

            await _service.Eliminar(request.Id, ct);

            _logger.LogInformation($"Almacén eliminado: {request.Id}");

            return request.Id;
        }
    }
}
