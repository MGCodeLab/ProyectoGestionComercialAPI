using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.UnidadMedida.ActualizarEstado;

public class ActualizarEstadoUnidadMedidaHandler
    : AuditableUpdateHandlerBase<ActualizarEstadoUnidadMedidaCommand, Unit>
{
    private readonly IUnidadMedidaService _service;

    public ActualizarEstadoUnidadMedidaHandler(
        IUnidadMedidaService service,
        ILogger<ActualizarEstadoUnidadMedidaHandler> logger)
        : base(null, logger)
    {
        _service = service;
    }

    public override async Task<Unit> Handle(ActualizarEstadoUnidadMedidaCommand request, CancellationToken cancellationToken)
    {
        var unidad = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (unidad == null)
            throw new NotFoundException($"Unidad de medida con id {request.Id} no encontrada");

        Logger.LogInformation("Actualizando estado de unidad de medida: {UnidadId} a {Activo}", request.Id, request.Activo);

        unidad.Activo = request.Activo;

        await UpdateAuditableEntity(
            unidad,
            async () => await _service.Actualizar(cancellationToken),
            () => Unit.Value,
            cancellationToken
        );

        Logger.LogInformation("Estado actualizado exitosamente: {UnidadId}", request.Id);

        return Unit.Value;
    }
}
