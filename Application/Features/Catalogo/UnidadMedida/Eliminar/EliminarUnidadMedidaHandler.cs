using Application.Exceptions;
using Application.Features.Catalogo.Moneda.Eliminar;
using Application.Interfaces;
using Domain.Catalogo;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.UnidadMedida.Eliminar;

public class EliminarUnidadMedidaHandler : IRequestHandler<EliminarUnidadMedidaCommand, Unit>
{
    private readonly IUnidadMedidaService _service;
    private readonly ILogger<EliminarUnidadMedidaHandler> _logger;

    public EliminarUnidadMedidaHandler(IUnidadMedidaService service, ILogger<EliminarUnidadMedidaHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<Unit> Handle(EliminarUnidadMedidaCommand request, CancellationToken cancellationToken)
    {
        var unidad = await _service.ObtenerPorId(request.Id, false, cancellationToken);
        if (unidad == null)
            throw new NotFoundException($"Unidad de medida con id {request.Id} no encontrada");

        var tieneDependencias = await _service.TieneDependencias(unidad, cancellationToken);
        if (tieneDependencias)
            throw new BadRequestException("Unidad de medida en uso en productos. Solo se permite deshabilitar mediante PATCH /inactivar");

        await _service.Eliminar(unidad, cancellationToken);

        _logger.LogInformation("Unidad de medida eliminada exitosamente: {UnidadId}", request.Id);
        return Unit.Value;
    }
}
