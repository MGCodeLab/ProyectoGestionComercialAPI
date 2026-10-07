using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.Pais.Actualizar;

public class ActualizarPaisHandler
    : AuditableUpdateHandlerBase<ActualizarPaisCommand, Unit>
{
    private readonly IPaisService _service;

    public ActualizarPaisHandler(
        IPaisService service,
        IMapper mapper,
        ILogger<ActualizarPaisHandler> logger)
        : base(mapper, logger)
    {
        _service = service;
    }

    public override async Task<Unit> Handle(ActualizarPaisCommand request, CancellationToken cancellationToken)
    {
        var pais = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (pais == null)
            throw new NotFoundException($"País con id {request.Id} no encontrado");

        Logger.LogInformation("Actualizando país: {PaisId}", request.Id);

        Mapper.Map(request, pais);

        await UpdateAuditableEntity(
            pais,
            async () => await _service.Actualizar(cancellationToken),
            () => Unit.Value,
            cancellationToken
        );

        Logger.LogInformation("País actualizado exitosamente: {PaisId}", request.Id);

        return Unit.Value;
    }
}
