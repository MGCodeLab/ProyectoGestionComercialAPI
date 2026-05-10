using MediatR;
using Microsoft.Extensions.Logging;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Features.Catalogo.UnidadMedida.ActualizarEstado;

public class ActualizarEstadoUnidadMedidaHandler : IRequestHandler<ActualizarEstadoUnidadMedidaCommand>
{
    private readonly IUnidadMedidaService _service;
    private readonly ILogger<ActualizarEstadoUnidadMedidaHandler> _logger;

    public ActualizarEstadoUnidadMedidaHandler(IUnidadMedidaService service, ILogger<ActualizarEstadoUnidadMedidaHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task Handle(ActualizarEstadoUnidadMedidaCommand request, CancellationToken cancellationToken)
    {
        var unidad = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (unidad == null)
            throw new NotFoundException($"Unidad de medida con id {request.Id} no encontrada");

        _logger.LogInformation("Actualizando estado de unidad de medida: {UnidadId} a {Activo}", request.Id, request.Activo);

        unidad.Activo = request.Activo;
        unidad.FechaActualizacion = DateTime.UtcNow;

        await _service.Actualizar(cancellationToken);

        _logger.LogInformation("Estado actualizado exitosamente: {UnidadId}", request.Id);
    }
}
