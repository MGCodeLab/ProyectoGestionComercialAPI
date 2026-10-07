using Application.Exceptions;
using Application.Handlers;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.CondicionPago.Actualizar;

public class ActualizarCondicionPagoHandler
    : AuditableUpdateHandlerBase<ActualizarCondicionPagoCommand, int>
{
    private readonly ICondicionPagoService _service;

    public ActualizarCondicionPagoHandler(
        ICondicionPagoService service,
        IMapper mapper,
        ILogger<ActualizarCondicionPagoHandler> logger)
        : base(mapper, logger)
    {
        _service = service;
    }

    public override async Task<int> Handle(ActualizarCondicionPagoCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Actualizando condición de pago: {Id}", request.Id);

        var condicion = await _service.ObtenerPorId(request.Id, cancellationToken);
        if (condicion == null)
            throw new NotFoundException($"Condición de pago con id {request.Id} no encontrada");

        Mapper.Map(request, condicion);

        return await UpdateAuditableEntity(
            condicion,
            async () => await _service.Actualizar(condicion, cancellationToken),
            () => condicion.Id,
            cancellationToken
        );
    }
}
