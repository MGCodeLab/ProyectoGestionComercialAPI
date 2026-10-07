using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.ModuloSistema.Actualizar;

public class ActualizarModuloSistemaHandler
    : AuditableUpdateHandlerBase<ActualizarModuloSistemaCommand, Unit>
{
    private readonly IModuloSistemaService _service;

    public ActualizarModuloSistemaHandler(
        IModuloSistemaService service,
        IMapper mapper,
        ILogger<ActualizarModuloSistemaHandler> logger)
        : base(mapper, logger)
    {
        _service = service;
    }

    public override async Task<Unit> Handle(ActualizarModuloSistemaCommand request, CancellationToken cancellationToken)
    {
        var modulo = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (modulo == null)
            throw new NotFoundException($"Módulo con id {request.Id} no encontrado");

        Logger.LogInformation("Actualizando módulo: {ModuloId}", request.Id);

        Mapper.Map(request, modulo);

        await UpdateAuditableEntity(
            modulo,
            async () => await _service.Actualizar(cancellationToken),
            () => Unit.Value,
            cancellationToken
        );

        Logger.LogInformation("Módulo actualizado exitosamente: {ModuloId}", request.Id);

        return Unit.Value;
    }
}
