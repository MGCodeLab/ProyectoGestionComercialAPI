using MediatR;
using Microsoft.Extensions.Logging;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Features.Catalogo.Pais.ActualizarEstado;

public class ActualizarEstadoPaisHandler : IRequestHandler<ActualizarEstadoPaisCommand, Unit>
{
    private readonly IPaisService _service;
    private readonly ILogger<ActualizarEstadoPaisHandler> _logger;

    public ActualizarEstadoPaisHandler(IPaisService service, ILogger<ActualizarEstadoPaisHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<Unit> Handle(ActualizarEstadoPaisCommand request, CancellationToken cancellationToken)
    {
        var pais = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (pais == null)
            throw new NotFoundException($"País con id {request.Id} no encontrado");

        pais.Activo = request.Activo;
        pais.FechaActualizacion = DateTime.UtcNow;
        await _service.Actualizar(cancellationToken);
        _logger.LogInformation("Estado de país actualizado: {PaisId}", request.Id);
        return Unit.Value;
    }
}
