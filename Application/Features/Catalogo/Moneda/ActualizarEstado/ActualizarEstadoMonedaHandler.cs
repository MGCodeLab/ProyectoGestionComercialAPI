using MediatR;
using Microsoft.Extensions.Logging;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Features.Catalogo.Moneda.ActualizarEstado;

public class ActualizarEstadoMonedaHandler : IRequestHandler<ActualizarEstadoMonedaCommand, Unit>
{
    private readonly IMonedaService _service;
    private readonly ILogger<ActualizarEstadoMonedaHandler> _logger;

    public ActualizarEstadoMonedaHandler(IMonedaService service, ILogger<ActualizarEstadoMonedaHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<Unit> Handle(ActualizarEstadoMonedaCommand request, CancellationToken cancellationToken)
    {
        var moneda = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (moneda == null)
            throw new NotFoundException($"Moneda con id {request.Id} no encontrada");

        moneda.Activo = request.Activo;
        moneda.FechaActualizacion = DateTime.UtcNow;
        await _service.Actualizar(cancellationToken);
        _logger.LogInformation("Estado de moneda actualizado: {MonedaId}", request.Id);
        return Unit.Value;
    }
}
