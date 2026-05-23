using MediatR;
using Microsoft.Extensions.Logging;
using Application.Exceptions;
using Application.Interfaces;

namespace Application.Features.Catalogo.Moneda.Eliminar;

public class EliminarMonedaHandler : IRequestHandler<EliminarMonedaCommand, Unit>
{
    private readonly IMonedaService _service;
    private readonly ILogger<EliminarMonedaHandler> _logger;

    public EliminarMonedaHandler(IMonedaService service, ILogger<EliminarMonedaHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<Unit> Handle(EliminarMonedaCommand request, CancellationToken cancellationToken)
    {
        var moneda = await _service.ObtenerPorId(request.Id, false, cancellationToken);
        if (moneda == null)
            throw new NotFoundException($"Moneda con id {request.Id} no encontrada");

        var tieneDependencias = await _service.TieneDependencias(moneda, cancellationToken);
        if (tieneDependencias)
            throw new BadRequestException("Moneda en uso en países o empresas. Solo se permite deshabilitar mediante PATCH /inactivar");

        await _service.Eliminar(moneda, cancellationToken);
        _logger.LogInformation("Moneda eliminada: {MonedaId}", request.Id);
        return Unit.Value;
    }
}
