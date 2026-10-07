using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.Moneda.ActualizarEstado;

public class ActualizarEstadoMonedaHandler
    : AuditableUpdateHandlerBase<ActualizarEstadoMonedaCommand, Unit>
{
    private readonly IMonedaService _service;

    public ActualizarEstadoMonedaHandler(
        IMonedaService service,
        ILogger<ActualizarEstadoMonedaHandler> logger)
        : base(null, logger)
    {
        _service = service;
    }

    public override async Task<Unit> Handle(ActualizarEstadoMonedaCommand request, CancellationToken cancellationToken)
    {
        var moneda = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (moneda == null)
            throw new NotFoundException($"Moneda con id {request.Id} no encontrada");

        moneda.Activo = request.Activo;

        await UpdateAuditableEntity(
            moneda,
            async () => await _service.Actualizar(cancellationToken),
            () => Unit.Value,
            cancellationToken
        );

        Logger.LogInformation("Estado de moneda actualizado: {MonedaId}", request.Id);

        return Unit.Value;
    }
}
