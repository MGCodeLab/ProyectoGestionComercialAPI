using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.Moneda.Actualizar;

public class ActualizarMonedaHandler
    : AuditableUpdateHandlerBase<ActualizarMonedaCommand, Unit>
{
    private readonly IMonedaService _service;

    public ActualizarMonedaHandler(
        IMonedaService service,
        IMapper mapper,
        ILogger<ActualizarMonedaHandler> logger)
        : base(mapper, logger)
    {
        _service = service;
    }

    public override async Task<Unit> Handle(ActualizarMonedaCommand request, CancellationToken cancellationToken)
    {
        var moneda = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (moneda == null)
            throw new NotFoundException($"Moneda con id {request.Id} no encontrada");

        Logger.LogInformation("Actualizando moneda: {MonedaId}", request.Id);

        Mapper.Map(request, moneda);

        await UpdateAuditableEntity(
            moneda,
            async () => await _service.Actualizar(cancellationToken),
            () => Unit.Value,
            cancellationToken
        );

        Logger.LogInformation("Moneda actualizada exitosamente: {MonedaId}", request.Id);

        return Unit.Value;
    }
}
