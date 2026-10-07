using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.UnidadMedida.Actualizar;

public class ActualizarUnidadMedidaHandler
    : AuditableUpdateHandlerBase<ActualizarUnidadMedidaCommand, Unit>
{
    private readonly IUnidadMedidaService _service;

    public ActualizarUnidadMedidaHandler(
        IUnidadMedidaService service,
        IMapper mapper,
        ILogger<ActualizarUnidadMedidaHandler> logger)
        : base(mapper, logger)
    {
        _service = service;
    }

    public override async Task<Unit> Handle(ActualizarUnidadMedidaCommand request, CancellationToken cancellationToken)
    {
        var unidad = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (unidad == null)
            throw new NotFoundException($"Unidad de medida con id {request.Id} no encontrada");

        Logger.LogInformation("Actualizando unidad de medida: {UnidadId}", request.Id);

        Mapper.Map(request, unidad);

        await UpdateAuditableEntity(
            unidad,
            async () => await _service.Actualizar(cancellationToken),
            () => Unit.Value,
            cancellationToken
        );

        Logger.LogInformation("Unidad de medida actualizada exitosamente: {UnidadId}", request.Id);

        return Unit.Value;
    }
}
