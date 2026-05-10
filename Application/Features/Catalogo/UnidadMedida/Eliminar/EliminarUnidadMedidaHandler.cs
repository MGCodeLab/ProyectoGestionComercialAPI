using MediatR;
using Microsoft.Extensions.Logging;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Features.Catalogo.UnidadMedida.Eliminar;

public class EliminarUnidadMedidaHandler : IRequestHandler<EliminarUnidadMedidaCommand>
{
    private readonly IUnidadMedidaService _service;
    private readonly ILogger<EliminarUnidadMedidaHandler> _logger;

    public EliminarUnidadMedidaHandler(IUnidadMedidaService service, ILogger<EliminarUnidadMedidaHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task Handle(EliminarUnidadMedidaCommand request, CancellationToken cancellationToken)
    {
        var unidad = await _service.ObtenerPorId(request.Id, false, cancellationToken);
        if (unidad == null)
            throw new NotFoundException($"Unidad de medida con id {request.Id} no encontrada");

        _logger.LogInformation("Eliminando unidad de medida: {UnidadId}", request.Id);

        await _service.Eliminar(unidad, cancellationToken);

        _logger.LogInformation("Unidad de medida eliminada exitosamente: {UnidadId}", request.Id);
    }
}
