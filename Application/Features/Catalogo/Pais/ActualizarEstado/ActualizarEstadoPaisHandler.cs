using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.Pais.ActualizarEstado;

public class ActualizarEstadoPaisHandler
    : AuditableUpdateHandlerBase<ActualizarEstadoPaisCommand, Unit>
{
    private readonly IPaisService _service;

    public ActualizarEstadoPaisHandler(
        IPaisService service,
        ILogger<ActualizarEstadoPaisHandler> logger)
        : base(null, logger)
    {
        _service = service;
    }

    public override async Task<Unit> Handle(ActualizarEstadoPaisCommand request, CancellationToken cancellationToken)
    {
        var pais = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (pais == null)
            throw new NotFoundException($"País con id {request.Id} no encontrado");

        pais.Activo = request.Activo;

        await UpdateAuditableEntity(
            pais,
            async () => await _service.Actualizar(cancellationToken),
            () => Unit.Value,
            cancellationToken
        );

        Logger.LogInformation("Estado de país actualizado: {PaisId}", request.Id);

        return Unit.Value;
    }
}
